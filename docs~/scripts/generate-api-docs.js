const fs = require('fs');
const path = require('path');
const { glob } = require('glob');
const { XmlDocument } = require('xmldoc');

/**
 * C#ソースファイルからXMLコメントとコード定義を抽出してMarkdownを生成
 */

class CSharpDocGenerator {
  constructor(sourceDir, outputDir) {
    this.sourceDir = sourceDir;
    this.outputDir = outputDir;
  }

  /**
   * C#ファイルを再帰的に探索
   */
  async findCSharpFiles() {
    const pattern = path.join(this.sourceDir, '**/*.cs').replace(/\\/g, '/');
    const files = await glob(pattern, {
      ignore: ['**/obj/**', '**/bin/**', '**/Editor/**', '**/Tests/**']
    });
    return files;
  }

  /**
   * XMLコメントブロックを抽出
   */
  extractXmlComments(lines, startIndex) {
    const xmlLines = [];
    let i = startIndex;

    // XMLコメント行を収集（/// で始まる行）
    while (i >= 0 && lines[i].trim().startsWith('///')) {
      xmlLines.unshift(lines[i].trim().replace(/^\/\/\/\s?/, ''));
      i--;
    }

    if (xmlLines.length === 0) return null;

    // XML文字列を構築
    const xmlString = xmlLines.join('\n');

    try {
      // <root>で囲んで有効なXMLにする
      const wrappedXml = `<root>${xmlString}</root>`;
      const doc = new XmlDocument(wrappedXml);

      return {
        summary: this.getXmlText(doc.childNamed('summary')),
        params: this.getXmlParams(doc),
        returns: this.getXmlText(doc.childNamed('returns')),
        remarks: this.getXmlText(doc.childNamed('remarks')),
        example: this.getXmlText(doc.childNamed('example'))
      };
    } catch (e) {
      // XMLパースに失敗した場合はnullを返す
      return null;
    }
  }

  /**
   * XML要素からテキストを取得
   */
  getXmlText(element) {
    if (!element) return '';
    return element.val.trim();
  }

  /**
   * XMLからparamタグを全て取得
   */
  getXmlParams(doc) {
    const params = [];
    const paramElements = doc.childrenNamed('param');

    for (const param of paramElements) {
      params.push({
        name: param.attr.name || '',
        description: param.val.trim()
      });
    }

    return params;
  }

  /**
   * C#ファイルをパースしてクラス情報を抽出
   */
  parseFile(filePath) {
    const content = fs.readFileSync(filePath, 'utf-8');
    const lines = content.split('\n');

    const fileInfo = {
      path: filePath,
      relativePath: path.relative(this.sourceDir, filePath),
      namespace: '',
      classes: []
    };

    // 名前空間を抽出
    const namespaceMatch = content.match(/namespace\s+([\w.]+)/);
    if (namespaceMatch) {
      fileInfo.namespace = namespaceMatch[1];
    }

    let currentClass = null;

    for (let i = 0; i < lines.length; i++) {
      const line = lines[i];
      const trimmedLine = line.trim();

      // クラス定義を検出
      const classMatch = trimmedLine.match(/(public|internal|private|protected)?\s*(?:abstract|sealed|static)?\s*(?:partial)?\s*class\s+(\w+)/);
      if (classMatch) {
        const visibility = classMatch[1] || 'internal';
        const className = classMatch[2];

        // 継承クラスを抽出
        const inheritMatch = trimmedLine.match(/:\s*([\w,\s]+)/);
        const baseClasses = inheritMatch ? inheritMatch[1].split(',').map(s => s.trim()) : [];

        // XMLコメントを抽出
        const xmlDoc = this.extractXmlComments(lines, i - 1);

        currentClass = {
          type: 'class',
          name: className,
          visibility: visibility,
          baseClasses: baseClasses,
          xmlDoc: xmlDoc,
          methods: [],
          properties: [],
          fields: [],
          line: i + 1
        };

        fileInfo.classes.push(currentClass);
        continue;
      }

      // クラス内の場合のみメソッドやプロパティを検出
      if (!currentClass) continue;

      // メソッド定義を検出
      const methodMatch = trimmedLine.match(/(public|private|protected|internal)\s+(?:static\s+)?(?:virtual\s+)?(?:override\s+)?(?:abstract\s+)?(\w+(?:<[\w,\s]+>)?)\s+(\w+)\s*\(([^)]*)\)/);
      if (methodMatch && !trimmedLine.includes('=') && !trimmedLine.includes(';')) {
        const visibility = methodMatch[1];
        const returnType = methodMatch[2];
        const methodName = methodMatch[3];
        const params = methodMatch[4];

        // コンストラクタや演算子をスキップ
        if (methodName === currentClass.name || methodName.startsWith('operator')) {
          continue;
        }

        const xmlDoc = this.extractXmlComments(lines, i - 1);

        currentClass.methods.push({
          name: methodName,
          visibility: visibility,
          returnType: returnType,
          parameters: params,
          xmlDoc: xmlDoc,
          line: i + 1
        });
        continue;
      }

      // プロパティ定義を検出
      const propertyMatch = trimmedLine.match(/(public|private|protected|internal)\s+(?:static\s+)?(\w+(?:<[\w,\s]+>)?)\s+(\w+)\s*\{/);
      if (propertyMatch) {
        const visibility = propertyMatch[1];
        const propType = propertyMatch[2];
        const propName = propertyMatch[3];

        const xmlDoc = this.extractXmlComments(lines, i - 1);

        currentClass.properties.push({
          name: propName,
          visibility: visibility,
          type: propType,
          xmlDoc: xmlDoc,
          line: i + 1
        });
        continue;
      }

      // フィールド定義を検出
      const fieldMatch = trimmedLine.match(/(public|private|protected|internal)\s+(?:static\s+)?(?:readonly\s+)?(\w+(?:<[\w,\s]+>)?)\s+(\w+)\s*[;=]/);
      if (fieldMatch && !trimmedLine.includes('(')) {
        const visibility = fieldMatch[1];
        const fieldType = fieldMatch[2];
        const fieldName = fieldMatch[3];

        const xmlDoc = this.extractXmlComments(lines, i - 1);

        currentClass.fields.push({
          name: fieldName,
          visibility: visibility,
          type: fieldType,
          xmlDoc: xmlDoc,
          line: i + 1
        });
        continue;
      }
    }

    return fileInfo;
  }

  /**
   * クラス情報をMarkdownに変換
   */
  generateMarkdown(classInfo, fileInfo, packageName = null) {
    const lines = [];

    // ヘッダー
    lines.push(`# ${classInfo.name}`);
    lines.push('');

    // 名前空間
    if (fileInfo.namespace) {
      lines.push(`**Namespace:** \`${fileInfo.namespace}\``);
      lines.push('');
    }

    // 継承情報
    if (classInfo.baseClasses.length > 0) {
      lines.push(`**継承:** ${classInfo.baseClasses.map(c => `\`${c}\``).join(' → ')}`);
      lines.push('');
    }

    // 概要
    if (classInfo.xmlDoc && classInfo.xmlDoc.summary) {
      lines.push('## 概要');
      lines.push('');
      lines.push(classInfo.xmlDoc.summary);
      lines.push('');
    }

    // 備考
    if (classInfo.xmlDoc && classInfo.xmlDoc.remarks) {
      lines.push('## 備考');
      lines.push('');
      lines.push(classInfo.xmlDoc.remarks);
      lines.push('');
    }

    // パブリックフィールド
    const publicFields = classInfo.fields.filter(f => f.visibility === 'public');
    if (publicFields.length > 0) {
      lines.push('## フィールド');
      lines.push('');

      for (const field of publicFields) {
        lines.push(`### ${field.name}`);
        lines.push('');
        lines.push(`**型:** \`${field.type}\``);
        lines.push('');
        if (field.xmlDoc && field.xmlDoc.summary) {
          lines.push(field.xmlDoc.summary);
        } else {
          lines.push('_（説明なし）_');
        }
        lines.push('');
      }
    }

    // プロパティ
    const publicProps = classInfo.properties.filter(p => p.visibility === 'public');
    if (publicProps.length > 0) {
      lines.push('## プロパティ');
      lines.push('');

      for (const prop of publicProps) {
        lines.push(`### ${prop.name}`);
        lines.push('');
        lines.push(`**型:** \`${prop.type}\``);
        lines.push('');
        if (prop.xmlDoc && prop.xmlDoc.summary) {
          lines.push(prop.xmlDoc.summary);
        } else {
          lines.push('_（説明なし）_');
        }
        lines.push('');
      }
    }

    // メソッド
    const publicMethods = classInfo.methods.filter(m => m.visibility === 'public');
    if (publicMethods.length > 0) {
      lines.push('## メソッド');
      lines.push('');

      for (const method of publicMethods) {
        lines.push(`### ${method.name}`);
        lines.push('');
        lines.push('```csharp');
        lines.push(`${method.returnType} ${method.name}(${method.parameters})`);
        lines.push('```');
        lines.push('');

        // 概要
        if (method.xmlDoc && method.xmlDoc.summary) {
          lines.push(method.xmlDoc.summary);
          lines.push('');
        } else {
          lines.push('_（説明なし）_');
          lines.push('');
        }

        // パラメータ
        if (method.xmlDoc && method.xmlDoc.params.length > 0) {
          lines.push('**パラメータ:**');
          lines.push('');
          for (const param of method.xmlDoc.params) {
            lines.push(`- \`${param.name}\`: ${param.description}`);
          }
          lines.push('');
        }

        // 戻り値
        if (method.xmlDoc && method.xmlDoc.returns) {
          lines.push('**戻り値:**');
          lines.push('');
          lines.push(method.xmlDoc.returns);
          lines.push('');
        }

        // 例
        if (method.xmlDoc && method.xmlDoc.example) {
          lines.push('**例:**');
          lines.push('');
          lines.push('```csharp');
          lines.push(method.xmlDoc.example);
          lines.push('```');
          lines.push('');
        }
      }
    }

    // ソースコードへのリンク
    lines.push('---');
    lines.push('');

    // パッケージ名を判定
    if (!packageName) {
      // sourceDirからパッケージ名を推測
      if (this.sourceDir.includes('udon-radio-communications-redux-sf')) {
        packageName = 'jp.virtualaviation.udon-radio-communications-redux-sf';
      } else {
        packageName = 'jp.virtualaviation.udon-radio-communications-redux';
      }
    }

    lines.push(`**ソースコード:** [${fileInfo.relativePath}](https://github.com/VirtualAviationJapan/UdonRadioCommunications-Redux/blob/master/Packages/${packageName}/Runtime/${fileInfo.relativePath.replace(/\\/g, '/')})`);

    return lines.join('\n');
  }

  /**
   * 実行（クリーンアップなし）
   */
  async generateWithoutCleanup() {
    console.log('🔍 C#ファイルを検索中...');
    const csFiles = await this.findCSharpFiles();
    console.log(`   ${csFiles.length}個のファイルが見つかりました`);

    console.log('');
    console.log('📝 APIドキュメントを生成中...');

    const generatedFiles = [];

    for (const filePath of csFiles) {
      const fileInfo = this.parseFile(filePath);

      for (const classInfo of fileInfo.classes) {
        // publicクラスのみドキュメント化（XMLコメントがなくてもpublicならドキュメント化）
        if (classInfo.visibility !== 'public') continue;

        // publicメンバーが存在するか、またはXMLドキュメントがあるクラスのみ出力
        const hasPublicMembers =
          classInfo.methods.some(m => m.visibility === 'public') ||
          classInfo.properties.some(p => p.visibility === 'public') ||
          classInfo.fields.some(f => f.visibility === 'public');

        const hasClassDoc = classInfo.xmlDoc && classInfo.xmlDoc.summary;

        if (hasPublicMembers || hasClassDoc) {
          const markdown = this.generateMarkdown(classInfo, fileInfo);

          // ネームスペースベースのディレクトリを作成
          const namespaceDir = fileInfo.namespace
            ? path.join(this.outputDir, fileInfo.namespace)
            : this.outputDir;

          if (!fs.existsSync(namespaceDir)) {
            fs.mkdirSync(namespaceDir, { recursive: true });
          }

          const fileName = `${classInfo.name}.md`;
          const outputPath = path.join(namespaceDir, fileName);
          fs.writeFileSync(outputPath, markdown, 'utf-8');

          // ファイル情報を保存（リンク生成用に相対パスを含める）
          const relativeFilePath = fileInfo.namespace
            ? `${fileInfo.namespace}/${fileName}`
            : fileName;

          generatedFiles.push({
            fileName: relativeFilePath,
            namespace: fileInfo.namespace,
            className: classInfo.name
          });
          console.log(`   ✓ ${relativeFilePath}`);
        }
      }
    }

    return generatedFiles;
  }

  /**
   * 実行
   */
  async generate() {
    // 出力ディレクトリをクリーンアップ
    if (fs.existsSync(this.outputDir)) {
      fs.rmSync(this.outputDir, { recursive: true, force: true });
    }
    fs.mkdirSync(this.outputDir, { recursive: true });

    const generatedFiles = await this.generateWithoutCleanup();

    // インデックスファイルを生成
    this.generateIndex(generatedFiles);

    console.log('');
    console.log(`✅ ${generatedFiles.length}個のAPIドキュメントを生成しました`);
    console.log(`📁 出力先: ${this.outputDir}`);
  }

  /**
   * インデックスファイルを生成
   */
  generateIndex(fileList) {
    const lines = [];

    lines.push('# API リファレンス');
    lines.push('');
    lines.push('UdonRadioCommunications-Redux の API ドキュメント');
    lines.push('');

    // 名前空間ごとにグループ化
    const byNamespace = {};
    for (const { fileName, namespace, className } of fileList) {
      if (!byNamespace[namespace]) {
        byNamespace[namespace] = [];
      }
      byNamespace[namespace].push({ className, fileName });
    }

    // 名前空間ごとに出力
    for (const [namespace, classes] of Object.entries(byNamespace).sort()) {
      if (namespace) {
        lines.push(`## ${namespace}`);
        lines.push('');
      }

      for (const { className, fileName } of classes.sort((a, b) => a.className.localeCompare(b.className))) {
        lines.push(`- [${className}](./${fileName.replace('.md', '')})`);
      }
      lines.push('');
    }

    const indexPath = path.join(this.outputDir, 'index.md');
    fs.writeFileSync(indexPath, lines.join('\n'), 'utf-8');
    console.log('   ✓ index.md (APIリファレンス インデックス)');
  }
}

// スクリプト実行
(async () => {
  const packagesDir = path.join(__dirname, '../../Packages');
  const outputDir = path.join(__dirname, '../docs/api');

  // 出力ディレクトリをクリーンアップ（最初に1回だけ）
  console.log('🧹 出力ディレクトリをクリーンアップ中...');
  if (fs.existsSync(outputDir)) {
    fs.rmSync(outputDir, { recursive: true, force: true });
  }
  fs.mkdirSync(outputDir, { recursive: true });

  const allGeneratedFiles = [];

  // 各パッケージのRuntimeディレクトリを処理
  const packages = [
    'jp.virtualaviation.udon-radio-communications-redux',
    'jp.virtualaviation.udon-radio-communications-redux-sf'
  ];

  for (const packageName of packages) {
    const sourceDir = path.join(packagesDir, packageName, 'Runtime');

    if (!fs.existsSync(sourceDir)) {
      console.log(`⚠️  ${packageName}/Runtime が見つかりません。スキップします。`);
      continue;
    }

    console.log('');
    console.log(`📦 パッケージ: ${packageName}`);

    const generator = new CSharpDocGenerator(sourceDir, outputDir);
    const files = await generator.generateWithoutCleanup();
    allGeneratedFiles.push(...files);
  }

  // 全パッケージのインデックスを生成
  console.log('');
  console.log('📑 統合インデックスを生成中...');
  const tempGenerator = new CSharpDocGenerator('', outputDir);
  tempGenerator.generateIndex(allGeneratedFiles);

  console.log('');
  console.log(`✅ ${allGeneratedFiles.length}個のAPIドキュメントを生成しました`);
  console.log(`📁 出力先: ${outputDir}`);
})();
