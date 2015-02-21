tlbimp wiaaut.dll /out:Interop.Wia.dll /namespace:Interop.Wia /keyfile:"%~dp0\..\ScanComposer.snk" /machine:Agnostic
tlbimp wiascanprofiles.dll /out:Interop.Wia.ScanProfiles.dll /namespace:Interop.Wia.ScanProfiles /keyfile:"%~dp0\..\ScanComposer.snk" /machine:Agnostic
tlbimp wiavideo.dll /out:Interop.Wia.Video.dll /namespace:Interop.Wia.Video /keyfile:"%~dp0\..\ScanComposer.snk" /machine:Agnostic
