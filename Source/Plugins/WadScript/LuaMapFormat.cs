using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using MoonSharp.Interpreter;

using CodeImp.DoomBuilder.Map;
using CodeImp.DoomBuilder.Types;
using CodeImp.DoomBuilder.Config;

namespace CodeImp.DoomBuilder.DBXLua
{
    // referred to as MapFormat. static thing in lua
    // wrapper over formatinterface and gameconfiguration
    [MoonSharpUserData]
    public class LuaMapFormat
    {
        public static int GetMaxSidedefs()
        {
            return General.Map.FormatInterface.MaxSidedefs;
        }

        public static int GetVertexDecimals()
        {
            return General.Map.FormatInterface.VertexDecimals;
        }

        public static float GetMinLineLength()
        {
            return General.Map.FormatInterface.MinLineLength;
        }

        public static int GetMaxVertices()
        {
            return General.Map.FormatInterface.MaxVertices;
        }

        public static int GetMaxSectors()
        {
            return General.Map.FormatInterface.MaxSectors;
        }

        public static int GetMaxThings()
        {
            return General.Map.FormatInterface.MaxThings;
        }

        public static bool HasLinedefTag()
        {
            return General.Map.FormatInterface.HasLinedefTag;
        }

        public static bool HasThingTag()
        {
            return General.Map.FormatInterface.HasThingTag;
        }

        public static bool HasThingAction()
        {
            return General.Map.FormatInterface.HasThingAction;
        }

        public static bool HasCustomFields()
        {
            return General.Map.FormatInterface.HasCustomFields;
        }

        public static bool HasThingHeight()
        {
            return General.Map.FormatInterface.HasThingHeight;
        }

        public static bool HasActionArgs()
        {
            return General.Map.FormatInterface.HasActionArgs;
        }

        public static bool HasMixedActivations()
        {
            return General.Map.FormatInterface.HasMixedActivations;
        }

        public static bool HasBuiltInActivations()
        {
            return General.Map.FormatInterface.HasBuiltInActivations;
        }

        public static int GetMaxTag()
        {
            return General.Map.FormatInterface.MaxTag;
        }

        public static int GetMinTag()
        {
            return General.Map.FormatInterface.MinTag;
        }

        public static int GetMaxAction()
        {
            return General.Map.FormatInterface.MaxAction;
        }

        public static int GetMinAction()
        {
            return General.Map.FormatInterface.MinAction;
        }

        public static int GetMaxArgument()
        {
            return General.Map.FormatInterface.MaxArgument;
        }

        public static int GetMinArgument()
        {
            return General.Map.FormatInterface.MinArgument;
        }

        public static int GetMaxThingType()
        {
            return General.Map.FormatInterface.MinThingType;
        }

        public static double GetMaxCoordinate()
        {
            return General.Map.FormatInterface.MaxCoordinate;
        }

        public static double GetMinCoordinate()
        {
            return General.Map.FormatInterface.MinCoordinate;
        }

        public static int GetMaxThingAngle()
        {
            return General.Map.FormatInterface.MaxThingAngle;
        }

        public static int GetMinThingAngle()
        {
            return General.Map.FormatInterface.MinThingAngle;
        }

        public static Table GetDefaultLinedefFields()
        {
            return LuaTypeConversion.TableFromFieldInfos(General.Map.Config.LinedefFields);
        }

        public static Table GetDefaultSectorFields()
        {
            return LuaTypeConversion.TableFromFieldInfos(General.Map.Config.SectorFields);
        }

        public static Table GetDefaultSidedefFields()
        {
            return LuaTypeConversion.TableFromFieldInfos(General.Map.Config.SidedefFields);
        }

        public static Table GetDefaultThingFields()
        {
            return LuaTypeConversion.TableFromFieldInfos(General.Map.Config.ThingFields);
        }

        public static Table GetDefaultVertexFields()
        {
            return LuaTypeConversion.TableFromFieldInfos(General.Map.Config.VertexFields);
        }

        public static string GetGameName()
        {
            return General.Map.Config.Name;
        }

        public static string GetEngineName()
        {
            return General.Map.Config.EngineName;
        }

        public static string GetDefaultSaveCompiler()
        {
            return General.Map.Config.DefaultSaveCompiler;
        }

        public static string GetDefaultTestCompiler()
        {
            return General.Map.Config.DefaultTestCompiler;
        }

        public static float GetDefaultTextureScale()
        {
            return General.Map.Config.DefaultTextureScale;
        }

        public static float GetDefaultFlatScale()
        {
            return General.Map.Config.DefaultFlatScale;
        }

        public static bool HasScaledTextureOffsets()
        {
            return General.Map.Config.ScaledTextureOffsets;
        }

        public static string GetFormatInterface()
        {
            return General.Map.Config.FormatInterface;
        }

        public static string GetSoundLinedefFlag()
        {
            return General.Map.Config.SoundLinedefFlag;
        }

        public static string GetSingleSidedFlag()
        {
            return General.Map.Config.SingleSidedFlag;
        }

        public static string GetDoubleSidedFlag()
        {
            return General.Map.Config.DoubleSidedFlag;
        }

        public static string GetImpassableFlag()
        {
            return General.Map.Config.ImpassableFlag;
        }

        public static string GetUpperUnpeggedFlag()
        {
            return General.Map.Config.UpperUnpeggedFlag;
        }

        public static string GetLowerUnpeggedFlag()
        {
            return General.Map.Config.LowerUnpeggedFlag;
        }

        public static bool GetMixTexturesFlats()
        {
            return General.Map.Config.MixTexturesFlats;
        }

        public static bool HasGeneralizedActions()
        {
            return General.Map.Config.GeneralizedActions;
        }

        public static bool HasGeneralizedEffects()
        {
            return General.Map.Config.GeneralizedEffects;
        }

        public static int GetStart3DModeThingType()
        {
            return General.Map.Config.Start3DModeThingType;
        }

        public static int GetLinedefActivationsFilter()
        {
            return General.Map.Config.LinedefActivationsFilter;
        }

        public static string GetTestParameters()
        {
            return General.Map.Config.TestParameters;
        }

        public static bool GetTestShortPaths()
        {
            return General.Map.Config.TestShortPaths;
        }

        public static bool GetLineTagIndicatesSectors()
        {
            return General.Map.Config.LineTagIndicatesSectors;
        }

        public static string GetDecorateGames()
        {
            return General.Map.Config.DecorateGames;
        }

        public static string GetSkyFlatName()
        {
            return General.Map.Config.SkyFlatName;
        }

        public static int GetLeftBoundary()
        {
            return General.Map.Config.LeftBoundary;
        }

        public static int GetRightBoundary()
        {
            return General.Map.Config.RightBoundary;
        }

        public static int GetTopBoundary()
        {
            return General.Map.Config.TopBoundary;
        }

        public static int GetBottomBoundary()
        {
            return General.Map.Config.BottomBoundary;
        }

        public static bool HasDoomLightLevels()
        {
            return General.Map.Config.DoomLightLevels;
        }

        public static List<String> GetDefaultThingFlags()
        {
            return new List<string>(General.Map.Config.DefaultThingFlags);
        }

        public static IDictionary<string,string> GetThingFlags()
        {
            return General.Map.Config.ThingFlags;
        }

        public static IDictionary<string, string> GetLinedefFlags()
        {
            return General.Map.Config.LinedefFlags;
        }
    } // LuaMapFormat
}// ns
