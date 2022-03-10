---@class Game
---@field public CurrentPlayer Player
---@field public AllowedToPlayPlayer Player
---@field public Player1 Player
---@field public Player2 Player
Game = {}
--- Documentation a venir
---@param playerToWin Player Documentation a venir
function Game.MakeWin (playerToWin) end

--- Documentation a venir
---@return boolean Documentation a venir
function Game.TryEndPlayerTurn () end

--- Documentation a venir
---@param effectowner Player Documentation a venir
---@param card Card Documentation a venir
---@param discardSource (CardPile) | nil Documentation a venir
---@param discardGoal (DiscardPile) | nil Documentation a venir
---@return boolean Documentation a venir
---@overload fun(effectowner:Player,card:Card)
function Game.PlayCard (effectowner,card,discardSource,discardGoal) end

--- Documentation a venir
---@param effectowner Player Documentation a venir
---@param card Card Documentation a venir
---@return boolean Documentation a venir
function Game.PlayCardEffect (effectowner,card) end

--- Documentation a venir
---@param card Card Documentation a venir
---@return Player Documentation a venir
function Game.GetCurrentOwner (card) end

--- Documentation a venir
---@param card Card Documentation a venir
---@return CardPile Documentation a venir
function Game.GetPileOf (card) end

--- Documentation a venir
---@param player Player Documentation a venir
---@param card Card Documentation a venir
function Game.RevealCard (player,card) end

--- Documentation a venir
---@param player Player Documentation a venir
---@param artefact Artefact Documentation a venir
function Game.ActivateArtifact (player,artefact) end

--- Documentation a venir
---@param player Player Documentation a venir
---@param card Card Documentation a venir
---@param upgrade boolean Documentation a venir
---@return boolean Documentation a venir
function Game.CanPlay (player,card,upgrade) end

--- Documentation a venir
---@param player Player Documentation a venir
---@param cards IEnumerable<Card> | Card[] Documentation a venir
---@return Card Documentation a venir
function Game.ChooseBetween (player,cards) end

--- Documentation a venir
---@param nom string Documentation a venir
---@param description string Documentation a venir
---@param imageId (number | nil) | nil Documentation a venir
---@param effect (fun()) | nil Documentation a venir
---@return Card Documentation a venir
---@overload fun(nom:string,description:string)
function Game.MakeVirtual (nom,description,imageId,effect) end

--- Documentation a venir
---@param name string Documentation a venir
---@param description string Documentation a venir
---@param imageId number Documentation a venir
---@param effect fun() Documentation a venir
---@return Card Documentation a venir
function Game.CreateNewCard (name,description,imageId,effect) end



---@class Player
---@field public Deck CardPile
---@field public Hand CardPile
---@field public Discard DiscardPile
---@field public Id number
---@field public Artefacts Artefact[]
---@field public ActionPoints EventProperty<Player,number,ActionPointsEditEvent>
---@field public MaxActionPoints EventProperty<Player,number,MaxActionPointsEditEvent>
---@field public OtherPlayer Player
---@field public Cards IEnumerable<Card>
Player = {}
--- Documentation a venir
function Player.DrawCard () end

--- Documentation a venir
---@param card Card Documentation a venir
---@return boolean Documentation a venir
function Player.HasCard (card) end

--- Documentation a venir
function Player.LoopDeck () end

--- Documentation a venir
---@param obj Object | any Documentation a venir
---@return boolean Documentation a venir
function Player.Equals (obj) end



---@class ITargetable
ITargetable = {}


---@class Target
---@field public TargetType number
---@field public IsAutomatic boolean
---@field public Name string
Target = {}
--- Documentation a venir
---@param card Card Documentation a venir
---@return boolean Documentation a venir
function Target.IsValidTarget (card) end

--- Documentation a venir
---@return ITargetable Documentation a venir
function Target.GetAutomaticTarget () end



---@class TargetTypes
---@field public Player TargetTypes
---@field public Card TargetTypes


---@class EventProperty<S,T,ET>
---@field public Value T
EventProperty = {}
--- Documentation a venir
---@param newVal T Documentation a venir
---@return T Documentation a venir
function EventProperty.TryChangeValue (newVal) end

--- Documentation a venir
---@return string Documentation a venir
function EventProperty.ToString () end

--- Documentation a venir
---@param value T Documentation a venir
function EventProperty.StealthChange (value) end



---@class ActionPointsEditEvent : CancellableEvent
---@field public Player Player
---@field public OldPointCount number
---@field public NewPointCount number
ActionPointsEditEvent = {}


---@class CancellableEvent : Event
---@field public Cancelled boolean
CancellableEvent = {}


---@class Event
Event = {}


---@class MaxActionPointsEditEvent : CancellableEvent
---@field public Player Player
---@field public OldMaxPointCount number
---@field public NewMaxPointCount number
MaxActionPointsEditEvent = {}


---@class ChainOpportunityEvent : CancellableEvent
---@field public Chainer Player
ChainOpportunityEvent = {}


---@class DeckLoopEvent : Event
---@field public Player Player
DeckLoopEvent = {}


---@class EndTurnEvent : CancellableEvent
---@field public Player Player
EndTurnEvent = {}


---@class StartTurnEvent : Event
---@field public Player Player
StartTurnEvent = {}


---@class CardDeleteEvent : TransferrableCardEvent
CardDeleteEvent = {}


---@class CardEffectPlayEvent : TransferrableCardEvent
---@field public WhoPlayed Player
CardEffectPlayEvent = {}


---@class CardEvent : CancellableEvent
---@field public Card Card
CardEvent = {}


---@class CardMarkUpgradeEvent : CardEvent
CardMarkUpgradeEvent = {}


---@class CardMovePileEvent : CardEvent
---@field public SourcePile CardPile
---@field public SourceIndex number
---@field public DestPile CardPile
---@field public DestIndex number
CardMovePileEvent = {}


---@class CardPlayEvent : TransferrableCardEvent
---@field public WhoPlayed Player
CardPlayEvent = {}


---@class CardUnMarkUpgradeEvent : CardEvent
CardUnMarkUpgradeEvent = {}


---@class TransferrableCardEvent : CardEvent
---@field public Card Card
TransferrableCardEvent = {}


---@class TargetingEvent : Event
---@field public TargetData Target
---@field public ResolvedTarget ITargetable
TargetingEvent = {}


---@class CardImageIdChangeEvent : CardPropertyChangeEvent<number>
CardImageIdChangeEvent = {}


---@class CardCostChangeEvent : CardPropertyChangeEvent<number>
CardCostChangeEvent = {}


---@class CardDescriptionChangeEvent : CardPropertyChangeEvent<string>
CardDescriptionChangeEvent = {}


---@class CardLevelChangeEvent : CardPropertyChangeEvent<number>
CardLevelChangeEvent = {}


---@class CardNameChangeEvent : CardPropertyChangeEvent<string>
CardNameChangeEvent = {}


---@class CardPropertyChangeEvent<T> : CardEvent
---@field public OldValue T
---@field public NewValue T
CardPropertyChangeEvent = {}


---@class CardKeywordAddEvent : CardKeywordEvent
CardKeywordAddEvent = {}


---@class CardKeywordEvent : CardEvent
---@field public Keyword Keyword
CardKeywordEvent = {}


---@class CardKeywordRemoveEvent : CardKeywordEvent
CardKeywordRemoveEvent = {}


---@class CardKeywordTriggerEvent : CardKeywordEvent
CardKeywordTriggerEvent = {}


---@class ArtefactActivateEvent : ArtefactEvent
ArtefactActivateEvent = {}


---@class ArtefactChargeEditEvent : ArtefactEvent
---@field public NewChargeCount number
---@field public OldChargeCount number
ArtefactChargeEditEvent = {}


---@class ArtefactEvent : CancellableEvent
---@field public Artefact Artefact
ArtefactEvent = {}


---@class Artefact
---@field public Name string
---@field public MaxCharge number
---@field public CurrentCharge EventProperty<Artefact,number,ArtefactChargeEditEvent>
Artefact = {}
--- Documentation a venir
---@param game Game Documentation a venir
---@return boolean Documentation a venir
function Artefact.CanBeActivated (game) end



---@class Card
---@field public Name EventProperty<Card,string,CardNameChangeEvent>
---@field public EffectId string
---@field public IsVirtual boolean
---@field public MaxLevel number
---@field public Cost EventProperty<Card,number,CardCostChangeEvent>
---@field public CurrentLevel EventProperty<Card,number,CardLevelChangeEvent>
---@field public Description EventProperty<Card,string,CardDescriptionChangeEvent>
---@field public ImageId EventProperty<Card,number,CardImageIdChangeEvent>
---@field public Id number
---@field public IsMaxLevel boolean
Card = {}
--- Documentation a venir
---@param virtualThis (Card) | nil Documentation a venir
---@return Card Documentation a venir
---@overload fun()
function Card.Virtual (virtualThis) end

--- Documentation a venir
---@return Card Documentation a venir
function Card.Clone () end

--- Documentation a venir
---@param effectOwner Player Documentation a venir
---@return boolean Documentation a venir
function Card.CanBePlayed (effectOwner) end

--- Documentation a venir
---@return boolean Documentation a venir
function Card.Upgrade () end

--- Documentation a venir
---@return string Documentation a venir
function Card.ToString () end

--- Documentation a venir
---@param oldLevel number Documentation a venir
---@param newLevel number Documentation a venir
function Card.OnLevelChange (oldLevel,newLevel) end

--- Documentation a venir
---@param obj Object | any Documentation a venir
---@return boolean Documentation a venir
function Card.Equals (obj) end



---@class Keyword
---@field public Name string
Keyword = {}


---@class CardPile
---@field public Count number
---@field public IsEmpty boolean
---@field public [number] Card
CardPile = {}
--- Documentation a venir
---@param card Card Documentation a venir
---@return boolean Documentation a venir
function CardPile.Contains (card) end

--- Documentation a venir
---@param card Card Documentation a venir
---@return number Documentation a venir
function CardPile.IndexOf (card) end

--- Documentation a venir
---@param newCardPile CardPile Documentation a venir
---@param card Card Documentation a venir
---@param newPosition number Documentation a venir
---@return boolean Documentation a venir
function CardPile.MoveTo (newCardPile,card,newPosition) end

--- Documentation a venir
---@param card Card Documentation a venir
---@param newPosition number Documentation a venir
---@return boolean Documentation a venir
function CardPile.MoveInternal (card,newPosition) end

--- Documentation a venir
---@return string Documentation a venir
function CardPile.ToString () end



---@class DiscardPile : CardPile
DiscardPile = {}
--- Documentation a venir
---@param oldLocation CardPile Documentation a venir
---@param toUp Card Documentation a venir
---@return boolean Documentation a venir
function DiscardPile.MoveForUpgrade (oldLocation,toUp) end

--- Documentation a venir
---@param newCardPile CardPile Documentation a venir
---@param card Card Documentation a venir
---@param newPosition number Documentation a venir
---@return boolean Documentation a venir
function DiscardPile.MoveTo (newCardPile,card,newPosition) end

--- Documentation a venir
---@param card Card Documentation a venir
function DiscardPile.UnMarkForUpgrade (card) end

--- Documentation a venir
---@param card Card Documentation a venir
---@return boolean Documentation a venir
function DiscardPile.IsMarkedForUpgrade (card) end



---@class IEventHandler
---@field public EvenIfCancelled boolean
---@field public PostEvent boolean
---@field public EventType Type<Event>
IEventHandler = {}
--- Documentation a venir
---@param evt Event Documentation a venir
function IEventHandler.HandleEvent (evt) end



---@class List<T>
---@field public Capacity number
---@field public Count number
---@field public [number] T
List = {}
--- Documentation a venir
---@param item T Documentation a venir
function List.Add (item) end

--- Documentation a venir
---@param collection IEnumerable<T> | T[] Documentation a venir
function List.AddRange (collection) end

--- Documentation a venir
---@return ReadOnlyCollection<T> Documentation a venir
function List.AsReadOnly () end

--- Documentation a venir
---@param index number Documentation a venir
---@param count number Documentation a venir
---@param item T Documentation a venir
---@param comparer IComparer<T> Documentation a venir
---@return number Documentation a venir
function List.BinarySearch (index,count,item,comparer) end

--- Documentation a venir
---@param item T Documentation a venir
---@return number Documentation a venir
function List.BinarySearch (item) end

--- Documentation a venir
---@param item T Documentation a venir
---@param comparer IComparer<T> Documentation a venir
---@return number Documentation a venir
function List.BinarySearch (item,comparer) end

--- Documentation a venir
function List.Clear () end

--- Documentation a venir
---@param item T Documentation a venir
---@return boolean Documentation a venir
function List.Contains (item) end

--- Documentation a venir
---@param index number Documentation a venir
---@param array T[] Documentation a venir
---@param arrayIndex number Documentation a venir
---@param count number Documentation a venir
function List.CopyTo (index,array,arrayIndex,count) end

--- Documentation a venir
---@param array T[] Documentation a venir
---@param arrayIndex number Documentation a venir
function List.CopyTo (array,arrayIndex) end

--- Documentation a venir
---@return fun():T Documentation a venir
function List.__iterator () end

--- Documentation a venir
---@param item T Documentation a venir
---@return number Documentation a venir
function List.IndexOf (item) end

--- Documentation a venir
---@param index number Documentation a venir
---@param item T Documentation a venir
function List.Insert (index,item) end

--- Documentation a venir
---@param index number Documentation a venir
---@param collection IEnumerable<T> | T[] Documentation a venir
function List.InsertRange (index,collection) end

--- Documentation a venir
---@param item T Documentation a venir
---@param index number Documentation a venir
---@param count number Documentation a venir
---@return number Documentation a venir
function List.LastIndexOf (item,index,count) end

--- Documentation a venir
---@param item T Documentation a venir
---@return boolean Documentation a venir
function List.Remove (item) end

--- Documentation a venir
---@param index number Documentation a venir
function List.RemoveAt (index) end

--- Documentation a venir
---@param index number Documentation a venir
---@param count number Documentation a venir
function List.RemoveRange (index,count) end

--- Documentation a venir
function List.Reverse () end

--- Documentation a venir
---@param index number Documentation a venir
---@param count number Documentation a venir
function List.Reverse (index,count) end

--- Documentation a venir
function List.Sort () end

--- Documentation a venir
---@param comparer IComparer<T> Documentation a venir
function List.Sort (comparer) end

--- Documentation a venir
---@param index number Documentation a venir
---@param count number Documentation a venir
---@param comparer IComparer<T> Documentation a venir
function List.Sort (index,count,comparer) end

--- Documentation a venir
---@return T[] Documentation a venir
function List.ToArray () end

--- Documentation a venir
function List.TrimExcess () end

--- Documentation a venir
---@param array T[] Documentation a venir
function List.CopyTo (array) end

--- Documentation a venir
---@param index number Documentation a venir
---@param count number Documentation a venir
---@return List<T> Documentation a venir
function List.GetRange (index,count) end

--- Documentation a venir
---@param item T Documentation a venir
---@param index number Documentation a venir
---@return number Documentation a venir
function List.IndexOf (item,index) end

--- Documentation a venir
---@param item T Documentation a venir
---@param index number Documentation a venir
---@param count number Documentation a venir
---@return number Documentation a venir
function List.IndexOf (item,index,count) end

--- Documentation a venir
---@param item T Documentation a venir
---@return number Documentation a venir
function List.LastIndexOf (item) end

--- Documentation a venir
---@param item T Documentation a venir
---@param index number Documentation a venir
---@return number Documentation a venir
function List.LastIndexOf (item,index) end



---@class ReadOnlyCollection<T>
---@field public Count number
---@field public [number] T
ReadOnlyCollection = {}
--- Documentation a venir
---@param value T Documentation a venir
---@return boolean Documentation a venir
function ReadOnlyCollection.Contains (value) end

--- Documentation a venir
---@param value T Documentation a venir
---@return number Documentation a venir
function ReadOnlyCollection.IndexOf (value) end

--- Documentation a venir
---@param array T[] Documentation a venir
---@param index number Documentation a venir
function ReadOnlyCollection.CopyTo (array,index) end



---@class IEnumerable<T>
IEnumerable = {}


---@class IComparer<T>
IComparer = {}
--- Documentation a venir
---@param x T Documentation a venir
---@param y T Documentation a venir
---@return number Documentation a venir
function IComparer.Compare (x,y) end



---@class Object
Object = {}
--- Documentation a venir
---@param obj Object | any Documentation a venir
---@return boolean Documentation a venir
function Object.Equals (obj) end

--- Documentation a venir
---@param objA Object | any Documentation a venir
---@param objB Object | any Documentation a venir
---@return boolean Documentation a venir
function Object.Equals (objA,objB) end

--- Documentation a venir
---@param objA Object | any Documentation a venir
---@param objB Object | any Documentation a venir
---@return boolean Documentation a venir
function Object.ReferenceEquals (objA,objB) end

--- Documentation a venir
---@return number Documentation a venir
function Object.GetHashCode () end

--- Documentation a venir
---@return Type<Event> Documentation a venir
function Object.GetType () end

--- Documentation a venir
---@return string Documentation a venir
function Object.ToString () end





-- EFFETS


---@class Type<T:Event>

---@type Type<ActionPointsEditEvent>
T_ActionPointsEditEvent = --[[---@type Type<ActionPointsEditEvent>]] {}

---@type Type<CancellableEvent>
T_CancellableEvent = --[[---@type Type<CancellableEvent>]] {}

---@type Type<Event>
T_Event = --[[---@type Type<Event>]] {}

---@type Type<MaxActionPointsEditEvent>
T_MaxActionPointsEditEvent = --[[---@type Type<MaxActionPointsEditEvent>]] {}

---@type Type<ChainOpportunityEvent>
T_ChainOpportunityEvent = --[[---@type Type<ChainOpportunityEvent>]] {}

---@type Type<DeckLoopEvent>
T_DeckLoopEvent = --[[---@type Type<DeckLoopEvent>]] {}

---@type Type<EndTurnEvent>
T_EndTurnEvent = --[[---@type Type<EndTurnEvent>]] {}

---@type Type<StartTurnEvent>
T_StartTurnEvent = --[[---@type Type<StartTurnEvent>]] {}

---@type Type<CardDeleteEvent>
T_CardDeleteEvent = --[[---@type Type<CardDeleteEvent>]] {}

---@type Type<CardEffectPlayEvent>
T_CardEffectPlayEvent = --[[---@type Type<CardEffectPlayEvent>]] {}

---@type Type<CardEvent>
T_CardEvent = --[[---@type Type<CardEvent>]] {}

---@type Type<CardMarkUpgradeEvent>
T_CardMarkUpgradeEvent = --[[---@type Type<CardMarkUpgradeEvent>]] {}

---@type Type<CardMovePileEvent>
T_CardMovePileEvent = --[[---@type Type<CardMovePileEvent>]] {}

---@type Type<CardPlayEvent>
T_CardPlayEvent = --[[---@type Type<CardPlayEvent>]] {}

---@type Type<CardUnMarkUpgradeEvent>
T_CardUnMarkUpgradeEvent = --[[---@type Type<CardUnMarkUpgradeEvent>]] {}

---@type Type<TransferrableCardEvent>
T_TransferrableCardEvent = --[[---@type Type<TransferrableCardEvent>]] {}

---@type Type<TargetingEvent>
T_TargetingEvent = --[[---@type Type<TargetingEvent>]] {}

---@type Type<CardImageIdChangeEvent>
T_CardImageIdChangeEvent = --[[---@type Type<CardImageIdChangeEvent>]] {}

---@type Type<CardCostChangeEvent>
T_CardCostChangeEvent = --[[---@type Type<CardCostChangeEvent>]] {}

---@type Type<CardDescriptionChangeEvent>
T_CardDescriptionChangeEvent = --[[---@type Type<CardDescriptionChangeEvent>]] {}

---@type Type<CardLevelChangeEvent>
T_CardLevelChangeEvent = --[[---@type Type<CardLevelChangeEvent>]] {}

---@type Type<CardNameChangeEvent>
T_CardNameChangeEvent = --[[---@type Type<CardNameChangeEvent>]] {}

---@type Type<CardPropertyChangeEvent<any>>
T_CardPropertyChangeEvent = --[[---@type Type<CardPropertyChangeEvent<any>>]] {}

---@type Type<CardKeywordAddEvent>
T_CardKeywordAddEvent = --[[---@type Type<CardKeywordAddEvent>]] {}

---@type Type<CardKeywordEvent>
T_CardKeywordEvent = --[[---@type Type<CardKeywordEvent>]] {}

---@type Type<CardKeywordRemoveEvent>
T_CardKeywordRemoveEvent = --[[---@type Type<CardKeywordRemoveEvent>]] {}

---@type Type<CardKeywordTriggerEvent>
T_CardKeywordTriggerEvent = --[[---@type Type<CardKeywordTriggerEvent>]] {}

---@type Type<ArtefactActivateEvent>
T_ArtefactActivateEvent = --[[---@type Type<ArtefactActivateEvent>]] {}

---@type Type<ArtefactChargeEditEvent>
T_ArtefactChargeEditEvent = --[[---@type Type<ArtefactChargeEditEvent>]] {}

---@type Type<ArtefactEvent>
T_ArtefactEvent = --[[---@type Type<ArtefactEvent>]] {}


