import { readdir, readFile, stat, writeFile } from "node:fs/promises";
import path from "node:path";

const siteRoot = path.resolve(process.argv[2] ?? path.join(".docfx", "_site"));
const navPattern = /(<nav class="toc" id="toc">)([\s\S]*?)(<\/nav>)/;
const tocRelPattern = /<meta name="docfx:tocrel" content="([^"]+)">/;

let updated = 0;

for (const htmlPath of await listHtml(siteRoot)) {
  const html = await readFile(htmlPath, "utf8");
  const tocRel = tocRelPattern.exec(html)?.[1];

  if (!tocRel || !navPattern.test(html)) {
    continue;
  }

  const tocJsonPath = resolveTocJsonPath(htmlPath, tocRel);
  if (!isInside(siteRoot, tocJsonPath)) {
    continue;
  }

  let toc;
  try {
    toc = JSON.parse(await readFile(tocJsonPath, "utf8"));
  } catch {
    continue;
  }

  const nav = renderItems(toc.items ?? [], {
    htmlDir: path.dirname(htmlPath),
    htmlPath,
    tocDir: path.dirname(tocJsonPath),
    level: 1,
  });

  if (!nav.trim()) {
    continue;
  }

  const nextHtml = html.replace(navPattern, `$1\n${nav}\n            $3`);
  if (nextHtml !== html) {
    await writeFile(htmlPath, nextHtml, "utf8");
    updated++;
  }
}

console.log(`Injected sidebar TOC into ${updated} HTML file(s).`);

async function listHtml(root) {
  const entries = await readdir(root);
  const files = [];

  for (const entry of entries) {
    const fullPath = path.join(root, entry);
    const info = await stat(fullPath);

    if (info.isDirectory()) {
      files.push(...await listHtml(fullPath));
    } else if (entry.endsWith(".html")) {
      files.push(fullPath);
    }
  }

  return files;
}

function resolveTocJsonPath(htmlPath, tocRel) {
  const cleanTocRel = tocRel.split("#")[0].split("?")[0];
  const jsonRel = cleanTocRel.replace(/\.html$/i, ".json");
  return path.resolve(path.dirname(htmlPath), jsonRel);
}

function renderItems(items, context) {
  const lines = [`${indent(context.level)}<ul class="nav level${context.level}">`];

  for (const item of items) {
    const childContext = { ...context, level: context.level + 1 };
    const children = item.items?.length ? renderItems(item.items, childContext) : "";
    const isActive = isActiveItem(item, context) || children.includes('class="active"');
    const liClass = isActive ? ' class="active"' : "";
    const linkClass = isActive ? ' class="active"' : "";
    const name = escapeHtml(item.name ?? "");

    lines.push(`${indent(context.level + 1)}<li${liClass}>`);
    if (children) {
      lines.push(`${indent(context.level + 2)}<span class="expand-stub"></span>`);
    }

    if (item.href) {
      const href = escapeHtml(toPageRelativeHref(item.href, context));
      lines.push(`${indent(context.level + 2)}<a href="${href}" title="${name}"${linkClass}>${name}</a>`);
    } else {
      lines.push(`${indent(context.level + 2)}<a${linkClass}>${name}</a>`);
    }

    if (children) {
      lines.push(children);
    }

    lines.push(`${indent(context.level + 1)}</li>`);
  }

  lines.push(`${indent(context.level)}</ul>`);
  return lines.join("\n");
}

function toPageRelativeHref(href, context) {
  if (/^(\w+:)?\/\//.test(href)) {
    return href;
  }

  const { targetPath, suffix } = resolveHref(href, context);
  let relative = slash(path.relative(context.htmlDir, targetPath));
  if (!relative) {
    relative = path.basename(targetPath);
  }

  return relative + suffix;
}

function isActiveItem(item, context) {
  if (!item.href || /^(\w+:)?\/\//.test(item.href)) {
    return false;
  }

  const { targetPath } = resolveHref(item.href, context);
  return slash(path.resolve(targetPath)).toLowerCase() === slash(path.resolve(context.htmlPath)).toLowerCase();
}

function resolveHref(href, context) {
  const match = /^([^#?]*)(.*)$/.exec(href);
  const target = match?.[1] ?? href;
  const suffix = match?.[2] ?? "";

  return {
    targetPath: path.resolve(context.tocDir, decodeURIComponent(target)),
    suffix,
  };
}

function isInside(root, candidate) {
  const relative = path.relative(root, candidate);
  return !!relative && !relative.startsWith("..") && !path.isAbsolute(relative);
}

function escapeHtml(value) {
  return value
    .replace(/&/g, "&amp;")
    .replace(/</g, "&lt;")
    .replace(/>/g, "&gt;")
    .replace(/"/g, "&quot;");
}

function indent(level) {
  return "  ".repeat(level + 5);
}

function slash(value) {
  return value.replace(/\\/g, "/");
}
