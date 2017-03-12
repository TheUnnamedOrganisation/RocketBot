﻿using System;
using System.Collections.Generic;
using PoGo.NecroBot.Logic.PoGoUtils;
using POGOProtos.Data;
using POGOProtos.Enums;
using POGOProtos.Networking.Responses;
using PoGo.NecroBot.Logic.State;
using RocketBot2.Forms;
using System.Linq;

namespace RocketBot2.Helpers
{
    public class PokemonObject
    {
        private static bool _initialized;
        private static Session _session;
        public static Dictionary<PokemonId, int> CandyToEvolveDict = new Dictionary<PokemonId, int>();

        public PokemonObject(PokemonData pokemonData)
        {
            PokemonData = pokemonData;
        }

        public PokemonData PokemonData { get; }

        public ulong Id
        {
            get { return PokemonData.Id; }
        }

        public PokemonId PokemonId
        {
            get { return PokemonData.PokemonId; }
        }

        public int Cp
        {
            get { return PokemonData.Cp; }
        }

        public int IndividualAttack
        {
            get { return PokemonData.IndividualAttack; }
        }

        public int IndividualDefense
        {
            get { return PokemonData.IndividualDefense; }
        }

        public int IndividualStamina
        {
            get { return PokemonData.IndividualStamina; }
        }

        public double GetIV
        {
            get { return Math.Round(PokemonInfo.CalculatePokemonPerfection(PokemonData)/100, 2); }
        }

        public double GetLv
        {
            get { return PokemonInfo.GetLevel(PokemonData); }
        }

        public string Nickname
        {
            get { return PokemonData.Nickname; }
        }

        public string Move1
        {
            get { return _session.Translation.GetPokemonMovesetTranslation(PokemonData.Move1); }
        }

        public string Move2
        {
            get { return _session.Translation.GetPokemonMovesetTranslation(PokemonData.Move2); }
        }

        public int Candy
        {
            get
            {
                return PokemonInfo.GetCandy(_session, PokemonData);
            }
        }
        public int CandyToEvolve
        {
            get
            {
                if (CandyToEvolveDict.ContainsKey(PokemonData.PokemonId))
                {
                    return CandyToEvolveDict[PokemonData.PokemonId];
                }
                return 0;
            }
        }

        public int EvolveTimes
        {
            get
            {
                if (CandyToEvolve > 0)
                {
                    return Candy/CandyToEvolve;
                }
                return 0;
            }
        }

        public bool CanEvolve
        {
            get { return EvolveTimes > 0; }
        }

        public static async void Initilize(Session session)
        {
            if (!_initialized)
            {
                _initialized = true;
                _session = session;

                if (_session.Client.Download.ItemTemplates == null)
                    await _session.Client.Download.GetItemTemplates();

                var templates = _session.Client.Download.ItemTemplates.Where(x => x.PokemonSettings != null)
                        .Select(x => x.PokemonSettings)
                        .ToList();

                foreach (var t in templates)
                {
                    if (t != null)
                    {
                        CandyToEvolveDict.Add(t.PokemonId, t.CandyToEvolve);
                    }
                }
            }
        }
    }
}