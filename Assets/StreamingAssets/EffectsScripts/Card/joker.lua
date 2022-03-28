---@type number
max_level = 2
---@type number
image_id = 520

---@type string
name = "Joker"
---@type number
pa_cost = 2

---@type ChainMode
chain_mode = ChainMode.NoChain

local base_description = "Cette carte peut copier l'effet d'une de vos cartes"
description = base_description

local function card_filter()
    local listeSansVictoire = {}
    for card in --[[---@type fun:Card]]EffectOwner.Hand do
        if card.EffectId ~= "_victoire"
                and card.EffectId ~= "joker" -- sinon boucle
                and card.EffectId ~= "amelioration_temporaire" -- sinon boucle
                and card ~= This and card.CanBePlayed(EffectOwner) then
            listeSansVictoire[#listeSansVictoire + 1] = card
        end
    end
    for card in --[[---@type fun:Card]]EffectOwner.Deck do
        if card.EffectId ~= "_victoire"
                and card.EffectId ~= "joker" -- sinon boucle
                and card.EffectId ~= "amelioration_temporaire" -- sinon boucle
                and card ~= This and card.CanBePlayed(EffectOwner) then
            listeSansVictoire[#listeSansVictoire + 1] = card
        end
    end
    for card in --[[---@type fun:Card]]EffectOwner.Discard do
        if card.EffectId ~= "_victoire"
                and card.EffectId ~= "joker" -- sinon boucle
                and card.EffectId ~= "amelioration_temporaire" -- sinon boucle
                and card ~= This and card.CanBePlayed(EffectOwner) then
            listeSansVictoire[#listeSansVictoire + 1] = card
        end
    end
    print("taille listSansVict : " .. tostring(#listeSansVictoire))
    local random = GetRandomNumber(0, #listeSansVictoire)
    print("random : " .. tostring(random))
    return listeSansVictoire[random]
end

---@type Target[]
targets = {
    CreateTarget("Effet d'une carte aléatoire de ton deck", TargetTypes.Card, true, card_filter),
    CreateTarget("Effet de 2 cartes aléatoires de ton deck", TargetTypes.Card, true, card_filter)
}


--Joker : lv1 : Carte copie l'effet d'une carte aléatoirement parmis toutes ses cartes.
--        lv2 : pareil mais deux fois.
--precondition -> 2 cartes jouables différentes
function precondition()
    ---@type Card
    local old
    for card in --[[---@type fun:Card]]EffectOwner.Cards do
        if card.EffectId ~= "_victoire"
                and card.EffectId ~= "joker" -- sinon boucle
                and card.EffectId ~= "amelioration_temporaire" -- sinon boucle
                and card ~= This and card.CanBePlayed(EffectOwner) then
            if (This.CurrentLevel.Value == 1) then
                return true
            else
                old = card
            end
        end
    end
    for card in --[[---@type fun:Card]]EffectOwner.Cards do
        if card.EffectId ~= "_victoire"
                and card.EffectId ~= "joker" -- sinon boucle
                and card.EffectId ~= "amelioration_temporaire" -- sinon boucle
                and card ~= This and card.CanBePlayed(EffectOwner) and card ~= old then
            return true
        end
    end
    return false
end

function do_effect()
    Game.RevealCard(EffectOwner.OtherPlayer, This)
    if (This.CurrentLevel.Value == max_level) then
        --application des 2 effets cartes lvl2
        local effet1 = (--[[---@type Card]] AskForTarget(1)).Virtual()                   --var qui recuper l'effet 1
        local effet2 = (--[[---@type Card]] AskForTarget(2)).Virtual()                   --var qui recuper l'effet 2
        Game.RevealCard(EffectOwner, effet1)
        Game.RevealCard(EffectOwner.OtherPlayer, effet1)
        Game.PlayCardEffect(EffectOwner, effet1)                   --joue l'effet de la 1 carte
        Game.RevealCard(EffectOwner, effet2)
        Game.RevealCard(EffectOwner.OtherPlayer, effet2)
        Game.PlayCardEffect(EffectOwner, effet2)                   --joue l'effet de la 2 carte
    else
        local effet1 = (--[[---@type Card]] AskForTarget(1)).Virtual()
        Game.RevealCard(EffectOwner, effet1)
        Game.RevealCard(EffectOwner.OtherPlayer, effet1)
        Game.PlayCardEffect(EffectOwner, effet1)
    end
end

---@param old number
---@param new number
function on_level_change(old, new)
    if (new == max_level) then
        This.Description.TryChangeValue("Cette carte peut copier l'effet de 2 de vos cartes")
    else
        This.Description.TryChangeValue(base_description)
    end
end
