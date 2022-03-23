---@type number
max_level = 3

---@type number
image_id = 564

---@type string
name = "Pioche Critique"
---@type number
pa_cost = 2

---@type ChainMode
chain_mode = ChainMode.StartOrMiddleChain

local base_description = "Cette carte te permet d'en piocher une de ton deck avec un coût augmenté de 2"
description = base_description

local function card_filter()
    local Deck = EffectOwner.Deck
    return Deck[0]
end

---@type Target[]
targets = {
    CreateTarget("La carte piochée", TargetTypes.Card, true, card_filter),
}

-- fonction qui renvoie un booléen si la carte peut être jouée ou non
function precondition()
    return EffectOwner.Deck.Count > 0 and (
            EffectOwner.Hand.LimitSize == nil or
                    EffectOwner.Hand.Count < EffectOwner.Hand.LimitSize
    )
end

---@type Card | nil
last_card = nil
---@type number
last_cost = 0

function do_effect()
    local carte = --[[---@type Card]] AskForTarget(1)
    print("Carte à bouger :" .. carte.ToString())

    if last_card == carte then
        print("deux fois sur la meme carte")
        ---@type Card
        local last_card = --[[---@type Card]] last_card
        last_card.Cost.TryChangeValue(last_cost)
    end
    
    local currentCost = carte.Cost.Value

    last_card = carte
    last_cost = currentCost
    ---@param playEvent CardPlayEvent
    ---@param handler IEventHandler
    local resetCost = function(playEvent, handler)
        print("resetcost handler")
        if playEvent.Card == carte then
            print("inside")
            carte.Cost.TryChangeValue(currentCost)
            UnsubscribeTo(handler)
        end
    end

    local res = EffectOwner.Deck.MoveTo(EffectOwner.Hand, carte, 0)
    print("carte bougée : " .. tostring(res))
    SubscribeTo(T_CardPlayEvent, resetCost, false, true)

    if (This.CurrentLevel.Value == 1) then
        carte.Cost.TryChangeValue(math.min(currentCost + 2, EffectOwner.MaxActionPoints.Value))
    elseif (This.CurrentLevel.Value == max_level) then
        carte.Cost.TryChangeValue(math.max(0, carte.Cost.Value - 1))
    end
end

function on_level_change(old, new)
    if new == 1 then
        This.Description.TryChangeValue(base_description)
    elseif new == max_level then
        This.Description.TryChangeValue("Cette carte te permet d'en piocher une de ton deck avec un coût diminué de 1")
    else
        This.Description.TryChangeValue("Cette carte te permet d'en piocher une de ton deck")
    end
end 