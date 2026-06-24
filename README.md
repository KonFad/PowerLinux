# PowerLinux

The C# GUI wrapper of PowerShell.
The main script also install pwsh for linux.

You can download binary for arm64 in /bin,
but for x86_64 it needs build on this platform,
you can do it by yourself with dotnet 
(remember, in installer you get .NET sdk automatically)

# install

``` shell
sudo pacman -S jq wget
```

it's dependencies before installing.

Then run the installation script:
```
wget -qO- https://raw.githubusercontent.com/KonFad/PowerLinux/main/install | bash
```

# run && build

try this commands:

```
dotnet run 
```

```
// for x86_64
dotnet build
```

# Raw Binary (ARM, or aarch64)
```
./bin/Debug/net10.0/PowerLinux
```

# Support
Termux
Linux (standart like Alpine, Arch, Ubuntu...)

