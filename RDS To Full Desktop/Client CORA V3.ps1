#Port de communication.
$port1 = 442
$port2 = 444

#On recup l'ip du Tsclient
$sessionID = (Get-Process -PID $pid).SessionID
$CLIENTNAME = (Get-ItemProperty -path ("HKCU:\Volatile Environment\" +  $sessionID) -name "CLIENTNAME").CLIENTNAME

$pc1 = $CLIENTNAME 
$ippc1 = [System.Net.Dns]::GetHostAddresses($pc1)

<#Mode débug: URL en DUR
$texte1 = "C:\Program Files (x86)\CORAGHT\CoRa Activite\CORA_GHT_ACTIVITE.bat"
#>

$texte1 = $args[0]
$password = $args[1]

<# Exemple de commandes attendues par le script

La commande à executer doit etre passée en argument 0 et le password en argument 1

####
Exemple de commande simple: 

"C:\Program Files (x86)\google\Chrome\Application\chrome.exe"
####

Exemple de commande complexe exe + argument:

' "C:\Program Files (x86)\google\Chrome\Application\chrome.exe" http://tintin.com' "coucou"

####
DATA = ' "C:\Program Files (x86)\google\Chrome\Application\chrome.exe" http://tintin.com'
Password = "coucou"
Attention de bien respecter la presence des ' et des ".


#>

###ENVOI DU PASSWORD

#Création d'un objet EndPoint.
$endpointpwd = new-object System.Net.IPEndPoint ([IPAddress]$ippc1[0],$port2)
#Création d'un objet Socket UDP.
$socpwd = new-Object System.Net.Sockets.UdpClient
#Préparation du texte à l'envoi.
$encodepwd = [Text.Encoding]::ASCII.GetBytes($password)
#Envoie du message.
$envoiepwd = $socpwd.Send($encodepwd,$encodepwd.length,$endpointpwd)

##ENVOI DES DATA

#Création d'un objet EndPoint.
$endpoint1 = new-object System.Net.IPEndPoint ([IPAddress]$ippc1[0],$port1)
#Création d'un objet Socket UDP.
$socudp1 = new-Object System.Net.Sockets.UdpClient
#Préparation du text à l'envoi.
$encode1 = [Text.Encoding]::ASCII.GetBytes($texte1)
#Envoie du message.
$envoie1 = $socudp1.Send($encode1,$encode1.length,$endpoint1)
######

#Fermeture.
$socpwd.Close()
$socudp1.Close()


