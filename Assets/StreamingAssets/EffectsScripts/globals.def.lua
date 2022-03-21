-- propriétés

---Retourne la cible demandée
---@param i number l'indice de la cible demandée
---@return Card | Player La cible demandée
function AskForTarget(i)
end

---Crée une cible selon les parametres renseignés
---@param nom_cible string Le nom de la cible qui sera affiché au joueur
---@param type_cible TargetTypes le type de la cible
---@param est_automatique boolean est-ce que la cible est a résolution automatique ou manuelle
---@param filtre_carte (fun(card:Card):boolean)|(fun():Card|Player) Fonction de filtrage de cible ou de résolution de cible
---@return Target La cible créée
---@overload fun(nom_cible:string,type_cible:TargetTypes,est_automatique:boolean)
function CreateTarget(nom_cible, type_cible, est_automatique, filtre_carte)
end

---Verifie si toutes les cibles du tableau ont au moins un élément valide
---**Attention** : Invalide pour une cible automatique
---@param tableau_a_verifier number[] Le tableau contenant les indices des cibles a vérifier
---@return boolean Vrai si au moins un élément est ciblable pour chaqune des cibles demandés
function TargetsExists(tableau_a_verifier)
end

---Abonne l'écouteur *ecouteur* a l'évenement *type_evenement*
---@generic T : Event
---@param type_evenement Type<T> Le type d'évenement a écouter
---@param ecouteur fun(event:T,handler) La fonction appelée lorsque l'évenement survient
---@param meme_annule boolean | nil Est-ce que l'écouteur doit etre notifié des évenements annulés
---@param ecouter_post boolean | nil Est-ce que l'écouteur doit écouter la version Post de l'évenement
---@return IEventHandler Référence a l'écouteur
function SubscribeTo(type_evenement, ecouteur, meme_annule, ecouter_post)
end

---Désabonne l'écouteur donné
---@param ecouteur IEventHandler L'écouteur a désabonner 
function UnsubscribeTo(ecouteur)
end

---@type Card
This = --[[---@type Card]]{}

---@type Game
Game = --[[---@type Game]]{}

---@type Player
EffectOwner = --[[---@type Player]] {}

---@type TargetTypes
TargetTypes = --[[---@type TargetTypes]]{}

---@type ChainMode
ChainMode = --[[---@type ChainMode]]{}

---@type List<Event>
EventStack = --[[---@type List<Event>]] {}

---@return Type<Event>
function Event.GetType()
end

---@return string
function Event.ToString()
end

--- Retourne un nombre aléatoire entre min et max
---@param min number le nombre inférieur
---@param max number le nombre supérieur
---@return number Le nombre aléatoire entre min et max
function GetRandomNumber(min,max) end