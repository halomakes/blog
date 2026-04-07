class Song {
    hidden [string[]] $FfmpegOutput;
    hidden [string] $CuePath;

    [string] $Album;
    [string] $Artist;
    [string] $Title;
    [string] $OutputPath;
    [string] $SourcePath;
    [int] $Index;

    Song([System.IO.FileSystemInfo] $file, [int] $index) {
        $this.SourcePath = $file;
        $this.CuePath = "$($file.BaseName).wav"
        $this.OutputPath = "./Burn/$($this.CuePath)";
        $this.Index = $index;
    }

    [void] Convert() {
        $this.FfmpegOutput = (ffmpeg -i $this.SourcePath -ar 44100 -sample_fmt s16 -map_metadata 0:s:0 -y $this.OutputPath 2>&1);
        $this.Album = $this.GetMetaAttribute("album");
        $this.Artist = $this.GetMetaAttribute("artist");
        $this.Title = $this.GetMetaAttribute("title");
    }

    hidden [string] GetMetaAttribute($attribute) {
        if ([string]::IsNullOrWhiteSpace($this.FfmpegOutput)) {
            return $null;
        }
        [string] $line = $this.FfmpegOutput | Select-String "   $Attribute";
        if ([string]::IsNullOrWhiteSpace($line)) {
            return $null;
        }
        [string[]] $split = $line.Split(":");
        if ($null -eq $split -or $split.Length -lt 2) {
            return $null;
        }
        [string] $trimmed = $split[1].Trim();

        # Normalize the text using FormKD (Compatibility Decomposition, followed by Canonical Composition)
        $normalizedText = [System.Text.RegularExpressions.Regex]::Replace($trimmed, '\p{M}', '').Normalize([System.Text.NormalizationForm]::FormKD)

        # Remove diacritic marks
        return [System.Text.RegularExpressions.Regex]::Replace($normalizedText, '\p{M}', '')
    }

    [string[]] CreateCueSegment() {
        return "
FILE `"$($this.CuePath)`" WAVE
  TRACK $($this.Index.ToString('00')) AUDIO
    TITLE `"$($this.Title)`"
    PERFORMER `"$($this.Artist)`"
    INDEX 01 00:00:00";
    }
}


function ConvertTo-RedbookAudio {
    if (-not [bool] (Get-Command -ErrorAction Ignore -Type Application "ffmpeg")) {
        Write-Error "ffmpeg does not appear to be installed or is not registered in the path.";
        return;
    }

    # convert / resample for CD
    New-Item -Force -Type Directory "Burn" | Out-Null;

    [Song[]] $songs = @();
    [int] $index = 1;
    [System.IO.FileSystemInfo[]] $flacs = Get-ChildItem -Filter "*.flac";
    $flacs | Select-Object | ForEach-Object {
        Write-Progress -PercentComplete ($index * 100 / $flacs.Length ) -Activity "Converting Songs" -Status "Transcoding $_";
        [Song] $song = [Song]::new($_, $index);
        $songs += $song;
        $song.Convert();

        $index++;
    }

    # Create CUE File to burn
    [string] $albumName = $songs[0].Album;
    [string] $cueSheet = "TITLE `"$albumName`"
PERFORMER `"$($songs[0].Artist)`"";
    $songs | ForEach-Object {
        $cueSheet += $_.CreateCueSegment();
    }

    $cueSheet | Out-File -Force "./Burn/$albumName.cue";

    # launch IMGBurn if we can
    [string] $executablePath = "$(${env:ProgramFiles(x86)})\IMGBurn\ImgBurn.exe";
    if (Test-Path $executablePath) {
        $cueFile = Get-Item "./Burn/$albumName.cue";
        & $executablePath /mode WRITE /source $cueFile.FullName;
    }
}