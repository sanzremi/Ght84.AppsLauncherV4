##[Ps1 To Exe]
##
##Kd3HDZOFADWE8uK1
##Nc3NCtDXThU=
##Kd3HFJGZHWLWoLaVvnQnhQ==
##LM/RF4eFHHGZ7/K1
##K8rLFtDXTiW5
##OsHQCZGeTiiZ49I=
##OcrLFtDXTiW5
##LM/BD5WYTiiZ4tI=
##McvWDJ+OTiiZ4tI=
##OMvOC56PFnzN8u+Vs1Q=
##M9jHFoeYB2Hc8u+Vs1Q=
##PdrWFpmIG2HcofKIo2QX
##OMfRFJyLFzWE8uK1
##KsfMAp/KUzWJ0g==
##OsfOAYaPHGbQvbyVvnQX
##LNzNAIWJGmPcoKHc7Do3uAuO
##LNzNAIWJGnvYv7eVvnQX
##M9zLA5mED3nfu77Q7TV64AuzAgg=
##NcDWAYKED3nfu77Q7TV64AuzAgg=
##OMvRB4KDHmHQvbyVvnQX
##P8HPFJGEFzWE8tI=
##KNzDAJWHD2fS8u+Vgw==
##P8HSHYKDCX3N8u+Vgw==
##LNzLEpGeC3fMu77Ro2k3hQ==
##L97HB5mLAnfMu77Ro2k3hQ==
##P8HPCZWEGmaZ7/K1
##L8/UAdDXTlaDjo3c7TJ490bvVmE6e8CnlZOX66KA0++sqCDLX58BWxpyjiyc
##Kc/BRM3KXhU=
##
##
##fd6a9f26a06ea3bc99616d4851b372ba

##################################VARIABLES########################################

#Les ports d'écoute utilisés
#Le port pour les DATA
$port1 = 442
#Le port pour le mot de passe.
$port2 = 444

#Le mot de passe qui assure la sécurité de la connexion
$Password = "coucou"

#Mode de recupération des appels contextuels autorisés:
#1 = valeurs en dur dans le script
#2 = Valeur recupérées dans le fichier.ini
$ModeAC = "2"

#Les chaines de caractères autorisés lors des appels contextuels
#Chaines autorisées par ini (Si ModeAC = 1)
$CheminACautorises = ".\ACRDS.ini"
##ATTENTION NE PAS LAISSER D ESPACE OU DE LIGNES VIDE A LA FIN DU .INI 

#Chaines autorisées par valeur en dur. (Si modeAC = 2)
$ChainesAutorisees = "1","2","3","4"

#Emplacement du fichier de logs
$Logfile =  ".\ACRDS_" + (Get-Date -Format "MMMMyyyy") + ".log"

##########################FIN DES VARIABLES########################################

##############################DEBUT DU SCRIPT######################################
#Création du fichier de logs (1 ficher par mois)

if((Test-Path $Logfile) -eq $false) {

New-Item -ItemType File -Path $Logfile -Verbose

}

#création de la fonction de log
Function LogWrite
{
   Param ([string]$logstring)
   $Date = Get-Date 
   $logstring = -join($Date, $logstring)
   Add-content $Logfile -value ($logstring)
}

# On vide la variable
$texte1 = ""
$texte2 = ""

#Création d'un objet EndPoint.
Write-Host "Création du EndPoint 1"
LogWrite " Création du EndPoint 1"
$endpoint1 = new-object System.Net.IPEndPoint ([IPAddress]::Any,$port1)

#Création d'un objet EndPoint.
Write-Host "Création du EndPoint 2"
LogWrite " Création du EndPoint 2"
$endpoint2 = new-object System.Net.IPEndPoint ([IPAddress]::Any,$port2)

#On boucle
while (1) {
#Création d'un objet Socket UDP.
Write-Host "Ouverture du socket 1"
LogWrite " Ouverture du socket 1"
$socudp1= new-Object System.Net.Sockets.UdpClient $port1

#Création d'un objet Socket UDP.
Write-Host "Ouverture du socket 2"
LogWrite " Ouverture du socket 2"
$socudp2= new-Object System.Net.Sockets.UdpClient $port2

#On attend les données
Write-Host "En attente de reception de données"
LogWrite " En attente de reception de données"
$encode1 = $socudp1.Receive([ref]$endpoint1)

#On attend les données
$encode2 = $socudp2.Receive([ref]$endpoint2)

#Converti les données reçues
$texte1  = [Text.Encoding]::ASCII.GetString($encode1)

#Converti les données reçues
$texte2  = [Text.Encoding]::ASCII.GetString($encode2)

#On teste le mot de passe.
#Si il correspond on continue.
if ($texte2 -eq $Password) {
Write-Host "Mot de passe approuvé" -ForegroundColor Green
LogWrite " Mot de passe approuvé"

#On compare les chaines autorisées avec la chaine reçue

if ($ModeAC -eq "1") {
$ChainesAutorisees = $ChainesAutorisees
}

if ($ModeAC -eq "2") {

$ChainesAutorisees = (Get-Content $CheminACautorises)

}


foreach ($chaine in $ChainesAutorisees) {
Write-Host $chaine -ForegroundColor Blue
$result
$result = $texte1.IndexOf($chaine)

if ($result -eq "-1") {

Write-Host "La chaine: $chaine n'a pas été trouvée dans:  $texte1" -ForegroundColor Red
LogWrite " La chaine: $chaine n'a pas été trouvée dans:  $texte1" 
}

else {
Write-Host "La chaine: $chaine à été trouvée dans: $texte1" -ForegroundColor Green
LogWrite " La chaine: $chaine à été trouvée dans: $texte1" 

Write-Host "Lancement de l'appel contextuel: $texte1"
LogWrite " Lancement de l'appel contextuel: $texte1"
(cmd /c $texte1)


#On termine par un break pour sortir de la boucle et ne pas jouer l'action plusieurs fois.
break
}

}

}

else {
Write-Host "Mot de passe incorrect" -ForegroundColor Red
LogWrite " Mot de passe incorrect"

}


#Fermeture.
Write-Host "Fermeture des sockets"
LogWrite " Fermeture des sockets"
$socudp1.Close()
$socudp2.Close()

}

# Exemple de chaines attendues par l'outil:

# .\ClientCORAV3.ps1 '"C:\Program Files (x86)\google\Chrome\Application\chrome.exe" http://tintin.com' "coucou"


