const fs = require('fs');
const path = require('path');

// CHANGELOGをdocsディレクトリにコピー
fs.copyFileSync(
    path.join(__dirname, '../../CHANGELOG.md'),
    path.join(__dirname, '../docs/changelog.md')
);
