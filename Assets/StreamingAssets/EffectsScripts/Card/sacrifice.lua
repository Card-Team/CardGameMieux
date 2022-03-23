---@type number
max_level = 1

---@type number
image_id = 528

---@type string
name = "Sacrifice"
---@type number
pa_cost = 2

---@type ChainMode
chain_mode = ChainMode.StartChain

local base_description = "Enlever le marquage d'évolution d'une de vos cartes et réduisez le coût d'une carte de votre main"
description = base_description

local function card_filter(aCard)
    return EffectOwner.Discard.Contains(aCard)
            and This ~= aCard and EffectOwner.Discard.IsMarkedForUpgrade(aCard)
end

local function card_filter_main(aCard)
    return EffectOwner.Hand.Contains(aCard)
            and This ~= aCard
end

---@type Target[]
targets = {
    CreateTarget("la carte dont on veut enlever le marquage", TargetTypes.Card, false, card_filter),
    CreateTarget("la carte dont il faut réduire le coût", TargetTypes.Card, false, card_filter_main),
}

-- fonction qui renvoie un booléen si la carte peut être jouée ou non
function precondition()
    return TargetsExists({ 1, 2 }) and EffectOwner.Hand.Count > 0
end

function do_effect()

    local card = --[[---@type Card]] AskForTarget(1)
    EffectOwner.Discard.UnMarkForUpgrade(card)

    local costChange = --[[---@type Card]] AskForTarget(2)

    local currentCost = costChange.Cost.Value

    local resetCost = function(playEvent, handler)
        if playEvent == costChange then
            costChange.Cost.TryChangeValue(currentCost)
            UnsubscribeTo(handler)
        end
    end

    costChange.Cost.TryChangeValue(math.max(1, currentCost - 2))

    SubscribeTo(T_CardPlayEvent, resetCost, false, true)

end

