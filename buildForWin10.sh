# build standalone for win10 & drop all files in bin directory

read -r version < buildver.txt

dotnet publish --output bin --runtime win-x64 --configuration Release -p:PublishSingleFile=true -p:PublishTrimmed=true --self-contained true /p:Version="$version"