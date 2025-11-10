const fs = require('fs');
const path = require('path');
const { v4: uuidv4 } = require('uuid');
// coppy AiryBotCode.AiryDevBot and create new bot from it. with new name

try {
    require.resolve('uuid');
} catch (e) {
    console.error("The 'uuid' package is not installed. npm i uuid");
    process.exit(1);
}

const args = process.argv.slice(2);
const botName = args[0];

if (!botName) {
    console.error("Bot name cannot be empty. Please provide a name for the new bot.");
    process.exit(1);
}

const rootDir = path.resolve(__dirname, '..');
const sourceDir = path.join(rootDir, "Bots", "AiryBotCode.AiryDevBot");
const destinationDir = path.join(rootDir, "Bots", `AiryBotCode.${botName}`);

if (fs.existsSync(destinationDir)) {
    console.error(`A bot with the name '${botName}' already exists.`);
    process.exit(1);
}

// 1. Copy the directory
console.log(`Copying ${sourceDir} to ${destinationDir}...`);
fs.cpSync(sourceDir, destinationDir, { recursive: true });

// 2. Rename the .csproj file
const oldCsprojPath = path.join(destinationDir, "AiryBotCode.AiryDevBot.csproj");
const newCsprojPath = path.join(destinationDir, `AiryBotCode.${botName}.csproj`);
fs.renameSync(oldCsprojPath, newCsprojPath);
console.log(`Renamed ${path.basename(oldCsprojPath)} to ${path.basename(newCsprojPath)}`);

// 3. Update the solution file
const solutionFilePath = path.join(rootDir, "AiryBotCode.sln");
let solutionContent = fs.readFileSync(solutionFilePath, 'utf8');

const newGuid = uuidv4().toUpperCase();
const devBotGuid = "25E95C69-B157-A675-C050-5C2989530BDF"; // GUID of AiryBotCode.AiryDevBot

const newProjectEntry = `Project("{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}") = "AiryBotCode.${botName}", "Bots\\AiryBotCode.${botName}\\AiryBotCode.${botName}.csproj", "{${newGuid}}"
EndProject`;

// Add new project entry
solutionContent = solutionContent.replace(
    /Project\("{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}"\) = "AiryBotCode\.AiryDevBot", "Bots\\AiryBotCode\.AiryDevBot\\AiryBotCode\.AiryDevBot\.csproj", "{25E95C69-B157-A675-C050-5C2989530BDF}"\r?\nEndProject/,
    `$&` + `\n` + newProjectEntry
);

// Add new project configuration platforms
const projectConfigRegex = new RegExp(`\\{${devBotGuid}\\}\.(Debug|Release)\|(Any CPU|x64|x86)\\.(ActiveCfg|Build\.0) = (Debug|Release)\|(Any CPU|x64|x86)`, 'g');
const devBotConfigs = solutionContent.match(projectConfigRegex);
if (devBotConfigs) {
    const newConfigs = devBotConfigs.map(config => config.replace(devBotGuid, newGuid));
    solutionContent = solutionContent.replace(
        /(GlobalSection\(ProjectConfigurationPlatforms\) = postSolution)/,
        `$&` + `\n` + newConfigs.join('\n')
    );
}

// Add new nested project entry
const nestedProjectRegex = new RegExp(`\\{${devBotGuid}\\} = \\{28E4A04F-F57C-48E5-A248-AB24903209DC\}`);
const devBotNested = solutionContent.match(nestedProjectRegex);
if (devBotNested) {
    const newNested = devBotNested[0].replace(devBotGuid, newGuid);
    solutionContent = solutionContent.replace(
        /(GlobalSection\(NestedProjects\) = preSolution)/,
        `$&` + `\n` + newNested
    );
}

fs.writeFileSync(solutionFilePath, solutionContent);
console.log(`Updated ${path.basename(solutionFilePath)}`);

// 4. Update the Program.cs file
const programCsPath = path.join(destinationDir, "Program.cs");
let programCsContent = fs.readFileSync(programCsPath, 'utf8');
programCsContent = programCsContent.replace(/AiryDevBot/g, botName);
fs.writeFileSync(programCsPath, programCsContent);
console.log(`Updated ${path.basename(programCsPath)}`);

// 5. Rename and update the bot's class file
const oldBotClassFilePath = path.join(destinationDir, "Bots", "AiryDevBot.cs");
const newBotClassFilePath = path.join(destinationDir, "Bots", `${botName}.cs`);
fs.renameSync(oldBotClassFilePath, newBotClassFilePath);
console.log(`Renamed ${path.basename(oldBotClassFilePath)} to ${path.basename(newBotClassFilePath)}`);

let botClassContent = fs.readFileSync(newBotClassFilePath, 'utf8');
botClassContent = botClassContent.replace(/AiryDevBot/g, botName);
fs.writeFileSync(newBotClassFilePath, botClassContent);
console.log(`Updated ${path.basename(newBotClassFilePath)}`);

// 6. Update the ServiceRegistration.cs file
const serviceRegPath = path.join(destinationDir, "Registers", "ServiceRegistration.cs");
let serviceRegContent = fs.readFileSync(serviceRegPath, 'utf8');
serviceRegContent = serviceRegContent.replace(/AiryDevBot/g, botName);
fs.writeFileSync(serviceRegPath, serviceRegContent);
console.log(`Updated ${path.basename(serviceRegPath)}`);

console.log(`Bot '${botName}' created successfully!`);
