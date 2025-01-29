param (
    [string]$Command,
    [string]$Arg
)
# will coincidentally make all Write-Error calls error out the whole script
$ErrorActionPreference = "Stop"

switch ($Command) {
    "junction" {}
    "dev" {}
    "release" {}
    "publish" {}
    "vsPostBuild" {}
    "vsPreBuild" {}
    "" {
        Write-Host "USAGE: ./do.ps1 <junction|release>"
        return
    }
    default {
        Write-Error "Unknown command: $Command"
        return 1
    }
}
$DIR_RIMWORLD = "${PSScriptRoot}\..\.." | Resolve-Path
if (!(Test-Path "$DIR_RIMWORLD\RimWorldWin64.exe")) {
    Write-Error "-- Mod's root should be placed under a subdirectory in SteamApps/common/RimWorld"
}

$MOD = "AlarmClock"
$JUNCTION_TARGET = "${DIR_RIMWORLD}\Mods\${MOD}.dev"
$DIR_RELEASED_MOD = "${DIR_RIMWORLD}\Mods\${MOD}"
$DIR_MOD = "${PSScriptRoot}\mod"

# make the mod visible in mods directory for development
function Do_junction {
    if (Test-Path "${DIR_RELEASED_MOD}") {
        Write-Warning "-- Released mod folder ${DIR_RELEASED_MOD} exists; junction ${JUNCTION_TARGET} would emit conflict messages in RimWorld"
        if ($Arg -eq "--clean") {
            Remove-Item -Recurse "${DIR_RELEASED_MOD}"
            Write-Host "-- Existing folder removed"
        }
        Write-Information "-- Re-run with --clean to remove release directory"
    }
    if (Test-Path $JUNCTION_TARGET) {
        if ((Get-Item -Path $JUNCTION_TARGET -Force).LinkType -eq "Junction") {
            Write-Host "-- Junction ${JUNCTION_TARGET} already exists; do nothing"
            return
        }
        else {
            Write-Error "-- Path ${JUNCTION_TARGET} exists but is not a junction"
            return
        }
    }
    Write-Host "-- Create junction $JUNCTION_TARGET"
    New-Item -ItemType Junction -Path "$JUNCTION_TARGET" -Value "$DIR_MOD"
    return 0
}

function Do_dev {
    if ("${env:RIMWORLD}" -eq "") {
        Write-Warning "Cannot use 'dev': specify path to RimWorld, like `$env:RIMWORLD='E:\SteamLibrary\steamapps\common\RimWorld\RimWorldWin64.exe'"
        return
    }
    _removeInstalledMod
    Do_junction
    Write-Host "-- Build 'Release' config"
    & dotnet build -c Release
    & ${env:RIMWORLD}
}

# build & pack the mod for publishing from inside the game main menu
function Do_release {
    Write-Host "-- Remove dev junction"
    _unjunction

    _gitEnsureEverythingCommitted
    Write-Host "-- Build 'Release' config"
    & dotnet build -c Release

    if (Test-Path "./${MOD}") {
        Write-Host "-- Remove old release folder"
        Remove-Item -Recurse "./${MOD}"
    }
    Copy-Item -Recurse "./mod" "./${MOD}"
    Compress-Archive "./${MOD}" -DestinationPath "${MOD}.zip" -Force

    _removeInstalledMod

    Move-Item "./${MOD}" "${DIR_RELEASED_MOD}"
    Write-Host "-- SUCCESS! Start RimWorld, enable Devmode, open Mod, publish to Steam"
}

function Do_publish {
    $exe = "${PSScriptRoot}\..\uploader\RimworldModReleaseTool.exe"
    if (!(Test-Path "$exe")) {
        Write-Error "Missing executable ${exe}`nGet Mlie's awesome tool from https://github.com/emipa606/RimworldModdingHelpers/tree/master/SteamUpdateTool/Compiled"
        return
    }
    & "${exe}" "${DIR_RELEASED_MOD}"
}

# entrypoint for csproj
function Do_vsPostBuild {
    $to = "${DIR_MOD}\Assemblies\"
    Write-Host ".NET postBuild copy:"
    Write-Host $Arg
    Write-Host "->"
    Write-Host $to
    if (!(Test-Path "${to}")) {
        New-Item -ItemType Directory "${to}"
    }
    Copy-Item -Path "${Arg}" -Destination "${DIR_MOD}\Assemblies\"
}

function _unjunction {
    if (!(Test-Path $JUNCTION_TARGET)) {
        Write-Host "-- Junction ${JUNCTION_TARGET} does not exist; do nothing"
        return
    }
    if ((Get-Item -Path $JUNCTION_TARGET -Force).LinkType -eq "Junction") {
        # Remove-Item causes confirmation even with force somehow
        $junction = Get-Item -Path "${JUNCTION_TARGET}"
        $junction.Delete()
    }
    else {
        Write-Error "-- Path ${JUNCTION_TARGET} exists but is not a junction"
        return
    }
}

function _gitEnsureEverythingCommitted {
    Write-Host "-- Ensure no uncommitted changes in ${PWD}"
    if (!(Get-Command "git" -ErrorAction SilentlyContinue)) {
        Write-Warning "-- 'git' not available; do nothing and pray for the best"
        return
    }

    # courtesy of https://stackoverflow.com/a/3879077
    & git update-index -q --ignore-submodules --refresh
    & git diff-files --quiet --ignore-submodules -- .
    if (! $?) {
        Write-Host "-- Changes detected:"
        git diff-files --name-status -r --ignore-submodules --
        Write-Error "-- Unstaged changes in the working tree (see above)"
        return
    }

    & git diff-index --cached --quiet HEAD --ignore-submodules -- .
    if (! $?) {
        Write-Host "-- Changes detected:"
        & git diff-index --cached --name-status -r --ignore-submodules HEAD --
        Write-Error "-- Uncommitted changes in index (see above)"
        return
    }
    Write-Host "-- All good; now do git clean just to be sure :E"
    Write-Host "   (this is just interactive preview, even if you hit C nothing gets deleted)"
    & git clean --dry-run -x -i ./mod ./src
}

function _removeInstalledMod {
    if (Test-Path "${DIR_RELEASED_MOD}") {
        Write-Host "-- Remove old installed mod folder"
        Remove-Item -Recurse "${DIR_RELEASED_MOD}"
    }
}

& "Do_${Command}"
