// scripts/generate-dtos.js
const { execSync } = require("child_process");
const fs = require("fs");
const path = require("path");

function pascalCase(str) {
  return str.replace(/(^\w|_\w)/g, (match) =>
    match.replace("_", "").toUpperCase()
  );
}

const schemaDir = path.resolve(__dirname, "../libs/shared/schema");
const tsOutDir = path.resolve(__dirname, "../libs/shared/src");
const csOutDir = path.resolve(__dirname, "../apps/server/BrainRing.Shared");

if (!fs.existsSync(tsOutDir)) fs.mkdirSync(tsOutDir, { recursive: true });

fs.readdirSync(schemaDir).forEach((file) => {
  if (file.endsWith(".json")) {
    const name = path.basename(file, ".json");
    const pascalName = pascalCase(name);
    execSync(
      `npx quicktype --lang ts --src ${schemaDir}/${file} --out ${tsOutDir}/${name}.ts`
    );
    execSync(
      `npx quicktype --lang cs --namespace BrainRing.Shared --src ${schemaDir}/${file} --out ${csOutDir}/${pascalName}Dto.cs`
    );
  }
});
