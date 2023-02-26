using System;
using System.Collections.Generic;
namespace Chess.Fen
{
    [Serializable]
    public class FenExamples
    {
        [Serializable]
        public enum FenStringsEnum
        { 
            StartPosition,
            ScholarsMate, 
            FoolsMate, 
            SmotheredMate, 
            EpauletteMate,
            AnastasiasMate,
            BodensMate,
            OperaMate,
            RuyLopez,
            SicilianDefense,
            FrenchDefense,
            PircDefense,
            CaroKannDefense,
            EnglishOpening,
            KingsPawnOpening,
            QueensPawnOpening,
            ItalianGame,
            TwoKnightsDefense,
            ScotchGame,
            PetrovDefense,
            KasparovDeepBlue,
            FourRooks,
            EnPassantExample1,
            CheckExample1,
            CheckExample2
        }
        private static Dictionary<FenStringsEnum, string> positions = new Dictionary<FenStringsEnum, string>()
        {
            { FenStringsEnum.StartPosition, "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1" },
            { FenStringsEnum.ScholarsMate, "r1bqkbnr/pppp1ppp/2n5/4p3/2B1P3/5N2/PPPP1PPP/RNBQK2R b KQkq - 1 2" },
            { FenStringsEnum.FoolsMate, "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1" },
            { FenStringsEnum.SmotheredMate, "rnbqkbnr/pppp1ppp/8/4p3/6P1/5P2/PPPPP2P/RNBQKBNR b KQkq - 0 1" },
            { FenStringsEnum.EpauletteMate, "rnbqkbnr/pppp1ppp/8/4p3/5P2/5P2/PPPPP2P/RNBQKBNR b KQkq - 0 1" },
            { FenStringsEnum.AnastasiasMate, "rnbqkbnr/pp1ppppp/8/2p5/4P3/8/PPPP1PPP/RNBQKBNR w KQkq - 0 1" },
            { FenStringsEnum.BodensMate, "rnbqkbnr/pppppppp/8/8/3P4/8/PPP1PPPP/RNBQKBNR b KQkq - 0 1" },
            { FenStringsEnum.OperaMate, "rnbqkbnr/pppppppp/8/8/2P5/8/PP1PPPPP/RNBQKBNR b KQkq - 0 1" },
            { FenStringsEnum.SicilianDefense, "r1bqkbnr/pp1ppppp/2n5/2p5/4P3/5N2/PPPP1PPP/RNBQKB1R b KQkq - 1 2" },
            { FenStringsEnum.FrenchDefense, "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1" },
            { FenStringsEnum.PircDefense, "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1" },
            { FenStringsEnum.CaroKannDefense, "rnbqkbnr/pp1ppppp/8/2p5/2B5/8/PPPPPPPP/RNBQK1NR b KQkq - 1 2" },
            { FenStringsEnum.EnglishOpening, "rnbqkbnr/pppppppp/8/8/2P5/8/PP1PPPPP/RNBQKBNR b KQkq - 0 1" },
            { FenStringsEnum.KingsPawnOpening, "rnbqkbnr/pppppppp/8/8/2P5/8/PP1PPPPP/RNBQKBNR b KQkq - 0 1" },
            { FenStringsEnum.QueensPawnOpening, "rnbqkbnr/pppppppp/8/8/2P5/8/PP1PPPPP/RNBQKBNR b KQkq - 0 1" },
            { FenStringsEnum.RuyLopez, "r1bqkbnr/pp1ppppp/2n5/2p5/4P3/5N2/PPPP1PPP/RNBQKB1R b KQkq - 1 2" },
            { FenStringsEnum.ItalianGame, "r1bqkbnr/pp1ppppp/2n5/2p5/4P3/5N2/PPPP1PPP/RNBQKB1R b KQkq - 1 2"},
            { FenStringsEnum.TwoKnightsDefense, "r1bqkbnr/pp1ppppp/2n5/2p5/4P3/5N2/PPPP1PPP/RNBQKB1R b KQkq - 1 2"},
            { FenStringsEnum.ScotchGame, "r1bqkbnr/pp1ppppp/2n5/2p5/4P3/5N2/PPPP1PPP/RNBQKB1R b KQkq - 1 2"},
            { FenStringsEnum.PetrovDefense, "r1bqkbnr/pp1ppppp/2n5/2p5/4P3/5N2/PPPP1PPP/RNBQKB1R b KQkq - 1 2"},
            { FenStringsEnum.KasparovDeepBlue, "r3k2r/p1ppqpb1/bn2pnp1/3PN3/1p2P3/2N2Q1p/PPPBBPPP/R3K2R w KQkq - 0 1"},
            { FenStringsEnum.FourRooks, "4rrk1/ppp2ppp/8/8/8/8/PPP2PPP/4RRK1 w - - 0 1"},
            { FenStringsEnum.EnPassantExample1, "rnb1kbn1/ppp2ppr/8/3p4/3P4/4q1P1/PPP1P3/RNBQKBN1 b Qq d3 0 4"},
            { FenStringsEnum.CheckExample1, "r1b1k1n1/2p2pp1/8/1nbq4/8/2K1P3/1P1B4/1N5r w q - 2 12"},
            { FenStringsEnum.CheckExample2, "r1b1k1n1/n1p2pp1/8/2bq4/8/2K1P3/1P1B4/1N5r b q - 1 12"}
        };
        
        private static Dictionary<FenStringsEnum, string> summaries = new Dictionary<FenStringsEnum, string>()
        {
            { FenStringsEnum.StartPosition, "The starting position in a game of chess, also known as the 'initial position' or 'default position'." },
            { FenStringsEnum.ScholarsMate, "A quick checkmate that can occur in the opening moves of a chess game involving moves e4, d4, and Qh5." },
            { FenStringsEnum.FoolsMate, "A very quick checkmate that can occur in the opening moves of a chess game involving moves g4 and f3." },
            { FenStringsEnum.SmotheredMate, "A checkmate involving a knight and a queen trapping the opponent's king." },
            { FenStringsEnum.EpauletteMate, "A checkmate involving a rook and a queen trapping the opponent's king on the edge of the board." },
            { FenStringsEnum.AnastasiasMate, "A checkmate involving a queen and a rook trapping the opponent's king on the edge of the board." },
            { FenStringsEnum.BodensMate, "A checkmate involving a queen and a knight trapping the opponent's king." },
            { FenStringsEnum.OperaMate, "A checkmate involving a queen and two bishops trapping the opponent's king." },
            { FenStringsEnum.RuyLopez, "An opening in chess that starts with the moves e4 e5 Nf3 Nc6 Bb5." },
            { FenStringsEnum.SicilianDefense, "A chess opening played by Black, starting with the moves e5 and Nf6." },
            { FenStringsEnum.FrenchDefense, "A chess opening played by Black, starting with the moves e6 and d5." },
            { FenStringsEnum.PircDefense, "A chess opening played by Black, starting with the moves e6 and d6." },
            { FenStringsEnum.CaroKannDefense, "A chess opening played by Black, starting with the moves c6 and d6." },
            { FenStringsEnum.EnglishOpening, "A chess opening that starts with the moves Nf3 and d4." },
            { FenStringsEnum.KingsPawnOpening, "A chess opening that starts with the move e4." },
            { FenStringsEnum.QueensPawnOpening, "A chess opening that starts with the move d4." },
            { FenStringsEnum.ItalianGame, "The Italian Game is a chess opening starting with the moves e4 e5, Nf3 Nc6, Bc4."},
            { FenStringsEnum.TwoKnightsDefense, "The Two Knights Defense is a chess opening starting with the moves e4 e5, Nf3 Nc6, Nc3."},
            { FenStringsEnum.ScotchGame, "The Scotch Game is a chess opening starting with the moves e4 e5, Nf3 Nc6, d4."},
            { FenStringsEnum.PetrovDefense, "The Petrov Defense is a chess opening starting with the moves e4 e5, Nf3 Nf6."},
            { FenStringsEnum.KasparovDeepBlue, "This position is known as the 'Kasparov-Deep Blue' position. It is the position from the famous game between Garry Kasparov and the IBM supercomputer Deep Blue in 1997, where Kasparov resigned after making a mistake."},
            { FenStringsEnum.FourRooks, "This is the 'Four rooks' position where all the rooks are on the same file"},
            { FenStringsEnum.EnPassantExample1, "for testing 'En Passant'"},
            { FenStringsEnum.CheckExample1, "check test 1"},
            { FenStringsEnum.CheckExample2, "check test 2"}
        };

        public static string GetFenByName(FenStringsEnum name)
        {
            if (positions.ContainsKey(name))
            {
                return positions[name];
            }
            else
            {
                return "Position not found.";
            }
        }
        
        public static string GetSummaryByName(FenStringsEnum name)
        {
            if (summaries.ContainsKey(name))
            {
                return summaries[name];
            }
            else
            {
                return "Summary not found.";
            }
        }

    }
}