---@type number
max_level = 6
---@type number
image_id = 7

---@type string
name = "Transfert"
---@type number
pa_cost = 3

---@type ChainMode
chain_mode = ChainMode.StartOrMiddleChain

---@type string
description = "Transfère le niveau de cette carte à une autre"

--fonction qui recupere toute les cartes de sa main sauf celle ci est lui demande d'en selectionné une
---@param aCard Card
local function cardFilter(aCard)
    return EffectOwner.Hand.Contains(aCard)
            and This ~= aCard and not aCard.IsMaxLevel
end

---@type Target[]
targets = {
    CreateTarget("Transferer niveau", TargetTypes.Card, false, cardFilter),
}

function precondition()
    return TargetsExists({1})
end

function do_effect()

    local card = --[[---@type Card]] AskForTarget(1)--la carte
    local NiveauTransfert = This.CurrentLevel.Value

    card.CurrentLevel.TryChangeValue(math.min(card.MaxLevel, card.CurrentLevel.Value + NiveauTransfert))
    This.CurrentLevel.TryChangeValue(1)

end

