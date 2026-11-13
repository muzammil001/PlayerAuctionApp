const fs = require('node:fs');
const path = require('node:path');

const packageRoot = path.join('node_modules', '@fortawesome', 'fontawesome-free');
const publicRoot = path.join('wwwroot', 'lib', 'fontawesome');
const fontSource = path.join(packageRoot, 'webfonts');
const fontTarget = path.join(publicRoot, 'webfonts');
const cssTarget = path.join(publicRoot, 'css');

fs.mkdirSync(fontTarget, { recursive: true });
fs.mkdirSync(cssTarget, { recursive: true });
for (const file of fs.readdirSync(fontSource)) {
    fs.copyFileSync(path.join(fontSource, file), path.join(fontTarget, file));
}
fs.copyFileSync(path.join(packageRoot, 'css', 'all.min.css'), path.join(cssTarget, 'all.min.css'));

const interSource = path.join('node_modules', '@fontsource-variable', 'inter', 'files', 'inter-latin-wght-normal.woff2');
const fontsTarget = path.join('wwwroot', 'fonts');
fs.mkdirSync(fontsTarget, { recursive: true });
fs.copyFileSync(interSource, path.join(fontsTarget, 'inter-latin-variable.woff2'));
