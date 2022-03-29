---@type number
max_level = 1
---@type number
image_id = 11

---@type string
name = "Espion"
---@type number
pa_cost = 3

---@type ChainMode
chain_mode = ChainMode.StartChain

---@type string
description = "Voir une carte aléatoire de la main de l'adversaire"

--fonction qui recupere toute les cartes de sa main
local function cardFilter()
    -- carte choisis aleatoirement depuis sa main
    local OtherPlayerHand = EffectOwner.OtherPlayer.Hand
    local random = GetRandomNumber(0,OtherPlayerHand.Count)
    return OtherPlayerHand[random]
end

---@type Target[]
targets = {
    CreateTarget("carte qui fait voir", TargetTypes.Card, true, cardFilter),
}

function precondition()
    return EffectOwner.OtherPlayer.Hand.Count > 0
end

function do_effect()
    --on recupere la carte de l'adversaire
    local card = --[[---@type Card]] AskForTarget(1)                        --la carte
    Game.RevealCard(EffectOwner, card)                   -- fait voir la carte selectionner de la main de l'adversaire
end