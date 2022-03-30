---@type number
max_level = 1
---@type number
image_id = 5

---@type string
name = "Échange"
---@type number
pa_cost = 2

---@type ChainMode
chain_mode = ChainMode.StartChain

---@type string
description = "Echange cette carte avec une carte aléatoire de la main de votre adversaire."

local function card_filter()
    -- Verifier quand pas de carte
    local OtherHand = EffectOwner.OtherPlayer.Hand
    local listeSansVictoire = {}
    for card in --[[---@type fun:Card]]OtherHand do
        if card.EffectId ~= "_victoire" then
            listeSansVictoire[#listeSansVictoire + 1] = card
        end
    end
    local random = GetRandomNumber(1, #listeSansVictoire)
    return listeSansVictoire[random]
end

---@type Target[]
targets = {
    CreateTarget("La carte que l'on veut échanger", TargetTypes.Card, true, card_filter),
}


-- fonction qui renvoie un booléen si la carte peut être jouée ou non
function precondition()
    return EffectOwner.OtherPlayer.Hand.Count > 0
end

function do_effect()
    local theCard = --[[---@type Card]] AskForTarget(1)
    
    local thePile = Game.GetPileOf(theCard)
    -- TODO échanger leur position aussi ( donc pas 0)
    local indexFirst = thePile.IndexOf(theCard)
    local indexMe = EffectOwner.Hand.IndexOf(This)
    thePile.MoveTo(EffectOwner.Hand, theCard, indexMe)
    EffectOwner.Hand.MoveTo(EffectOwner.OtherPlayer.Hand, This, indexFirst)
end

