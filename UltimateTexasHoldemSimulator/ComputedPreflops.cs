﻿using BerldPokerEngine.Poker;

namespace UltimateTexasHoldemSimulator
{
    internal static class ComputedPreflops
    {
        internal static (double raise4Winnings, double raise3Winnings, double checkWinnings) Get(IEnumerable<Card> playerCards)
        {
            string key = GetFormatHoleCards(playerCards);
            return _values[key];
        }

        internal static string GetFormatHoleCards(IEnumerable<Card> cardsToFormat)
        {
            Card[] cards = cardsToFormat.OrderByDescending(c => c).ToArray();

            Card p1 = cards[0];
            Card p2 = cards[1];

            string suffix = string.Empty;

            if (p1.Rank != p2.Rank)
            {
                if (p1.Suit == p2.Suit)
                {
                    suffix = "s";
                }
                else
                {
                    suffix = "o";
                }
            }

            return $"{Rank.ToChar(p1.Rank)}{Rank.ToChar(p2.Rank)}{suffix}";
        }

        internal static Dictionary<string, (double raise4Winnings, double raise3Winnings, double checkWinnings)> _values = new()
        {
            { "AA", (75535105760, 60766638280, 45998170800) },
            { "KK", (68996689580, 55406232820, 41815776060) },
            { "QQ", (63318338350, 50764298710, 38210259070) },
            { "JJ", (57677763260, 46153921620, 34630079980) },
            { "TT", (52036640250, 41543836390, 31051032530) },
            { "99", (44731012620, 35477676040, 26225563488) },
            { "88", (37996711970, 29957541110, 21923351554) },
            { "AKs", (34784596980, 27634126920, 20697590718) },
            { "AQs", (32988618710, 26188766230, 19626790011) },
            { "AJs", (31243483125, 24786031385, 18584921050) },
            { "77", (31189816110, 24378569450, 17590442700) },
            { "ATs", (29561342430, 23435429730, 17575216587) },
            { "KQs", (27533161425, 21911498205, 17124744234) },
            { "KJs", (25747709080, 20475527920, 16100695042) },
            { "AKo", (24581734160, 18154742210, 11933670235) },
            { "66", (24329177870, 18756033630, 13268372386) },
            { "KTs", (24089971605, 19144500755, 15129729298) },
            { "AQo", (22666556615, 16612191025, 10790084002) },
            { "QJs", (21482421440, 17178536330, 14548721747) },
            { "AJo", (20803026900, 15113043810, 9675281560) },
            { "A9s", (20001692160, 14639787850, 9611914058) },
            { "QTs", (19792583185, 15820804965, 13592750366) },
            { "ATo", (19005488425, 13668570095, 8592435092) },
            { "A8s", (18225600780, 13214999270, 8592057295) },
            { "55", (17444321410, 13112867750, 9017134840) },
            { "KQo", (16422764935, 11616890935, 7771304308) },
            { "JTs", (16336754445, 13178709385, 12625383307) },
            { "A7s", (16097419470, 11489489810, 7382426881) },
            { "K9s", (14569121870, 10378811760, 7231391533) },
            { "KJo", (14515014680, 10081298220, 6683892989) },
            { "A5s", (14477794500, 10314983520, 6916685592) },
            { "A9o", (14005394310, 9486046040, 5295365789) },
            { "A6s", (13619004030, 9463365730, 5992749830) },
            { "KTo", (12742425855, 8656809935, 5650155935) },
            { "A4s", (12377869305, 8588128205, 5749550833) },
            { "A8o", (12067631115, 7925930325, 4183987229) },
            { "A3s", (10418932720, 6970389200, 4643434461) },
            { "Q9s", (10261296395, 7046002455, 5749305634) },
            { "K8s", (10102141435, 6614990275, 4385721217) },
            { "A7o", (9774613905, 6065604405, 2919782850) },
            { "44", (9681754490, 6735579310, 4417865754) },
            { "QJo", (9558239465, 6145619355, 4704795587) },
            { "K7s", (8472069300, 5309879270, 3506550259) },
            { "A2s", (8387246430, 5291691930, 3492825471) },
            { "K9o", (7782505940, 4505290430, 2437959557) },
            { "QTo", (7750425060, 4691836190, 3689546505) },
            { "A5o", (7520652395, 4291844785, 1962414056) },
            { "A6o", (7126772005, 3903872025, 1487387875) },
            { "J9s", (6666857955, 4291369565, 4786579947) },
            { "K6s", (6484621750, 3698737040, 2443249227) },
            { "Q8s", (5850790515, 3326264395, 2940781187) },
            { "A4o", (5231244145, 2408046935, 707547350) },
            { "K5s", (4596794465, 2166581635, 1481687790) },
            { "T9s", (3884055250, 2194448660, 4310190734) },
            { "JTo", (3650274195, 1448786765, 2313346985) },
            { "K8o", (3487399845, 961850285, -82931113) },
            { "A3o", (3092804325, 640909015, -486760083) },
            { "Q9o", (2779391595, 530611525, 549279324) },
            { "K4s", (2454500475, 405324705, 396798231) },
            { "J8s", (2252499655, 567878775, 2020335372) },
            { "33", (1855419870, 306120850, 156357836) },
            { "K7o", (1706464555, -469703955, -1036981793) },
            { "T8s", (-634879600, -1614181920, 1581804945) },
            { "Q7s", (1316765495, -488096215, 164656460) },
            { "98s", (-3575848275, -3911776755, 1071204936) },
            { "A2o", (875315480, -1192286250, -1726877029) },
            { "K3s", (461474455, -1239646885, -633353859) },
            { "Q6s", (-127403180, -1642927110, -589841933) },
            { "K6o", (-435757005, -2207483675, -2160287826) },
            { "J7s", (-2236170610, -3211449980, -722511109) },
            { "J9o", (-1468302220, -2832226390, -808579556) },
            { "87s", (-9956863245, -9091128095, -1106545517) },
            { "T7s", (-5136931990, -5405017490, -1120834909) },
            { "Q8o", (-1456313900, -2966478800, -1941573583) },
            { "97s", (-8043204415, -7673079835, -1503646096) },
            { "Q5s", (-2007004770, -3168812740, -1562063381) },
            { "K2s", (-1562831650, -2910198190, -1676374408) },
            { "T9o", (-4856000045, -5498558795, -1666729962) },
            { "K5o", (-2466370405, -3856629995, -3169425637) },
            { "Q4s", (-4162585750, -4940911820, -2610414315) },
            { "76s", (-15654694950, -13713084040, -2956779304) },
            { "J8o", (-5708444835, -6333589795, -3250062453) },
            { "J6s", (-6769476100, -7023662920, -3432557819) },
            { "Q3s", (-6170221220, -6597809880, -3610235614) },
            { "86s", (-14435802455, -12859799515, -3670267924) },
            { "22", (-6038443160, -6178569000, -3764690152) },
            { "T6s", (-9633165335, -9188763445, -3795776431) },
            { "T8o", (-9207575350, -9090645620, -4057995039) },
            { "J5s", (-8071606795, -8066089975, -4070587673) },
            { "96s", (-12523006585, -11444137325, -4123511207) },
            { "K4o", (-4799489795, -5775898045, -4321299602) },
            { "Q7o", (-5822982630, -6563705060, -4392064216) },
            { "Q2s", (-8210459235, -8281371975, -4623888177) },
            { "65s", (-20651425080, -17770781170, -4759146221) },
            { "98o", (-12781837605, -11983515295, -4952592117) },
            { "J4s", (-10233148285, -9843190025, -5087159035) },
            { "Q6o", (-7404331110, -7833935880, -5217291780) },
            { "K3o", (-6972583995, -7570681445, -5418377343) },
            { "75s", (-20148791970, -17495588010, -5541056624) },
            { "J7o", (-10027480195, -9894046735, -5659862043) },
            { "J3s", (-12247879615, -11506020285, -6064005465) },
            { "Q5o", (-9425379700, -9475756170, -6240792828) },
            { "54s", (-24462770475, -20877356565, -6271402238) },
            { "85s", (-18941129220, -16652674670, -6308245889) },
            { "T7o", (-13541229830, -12663662390, -6415330017) },
            { "T5s", (-14185057355, -13017237685, -6460598359) },
            { "K2o", (-9181125940, -9394542930, -6529653137) },
            { "95s", (-17042272595, -15247537025, -6785374663) },
            { "J2s", (-14296348840, -13196445810, -7052463504) },
            { "T4s", (-15755696410, -14300188240, -7118832401) },
            { "87o", (-19567741020, -17491485100, -7135869291) },
            { "97o", (-17078286385, -15525162565, -7172762134) },
            { "Q4o", (-11771893490, -11405944340, -7348707575) },
            { "64s", (-25476849615, -21841051455, -7380281811) },
            { "J6o", (-14392688595, -13488329925, -8031496626) },
            { "T3s", (-17770010010, -15962956430, -8059924970) },
            { "74s", (-25047771865, -21628440175, -8224666671) },
            { "Q3o", (-13959704640, -13212732270, -8408840754) },
            { "J5o", (-15816027825, -14633369615, -8722849935) },
            { "T6o", (-17867618530, -16228157320, -8741190131) },
            { "53s", (-29221915675, -24897964155, -8886504076) },
            { "84s", (-23876461595, -20814688565, -9009594180) },
            { "T2s", (-19819009745, -17654098025, -9016233897) },
            { "76o", (-25629620680, -22408901830, -9038914544) },
            { "86o", (-23875918550, -21040379010, -9360716878) },
            { "96o", (-21386931785, -19075971355, -9439757934) },
            { "Q2o", (-16184286085, -15049682695, -9485606248) },
            { "94s", (-22008321820, -19433329700, -9490917407) },
            { "J4o", (-18168100615, -16568233705, -9796145966) },
            { "43s", (-31712254665, -26947387545, -9914355426) },
            { "63s", (-30337310165, -25946487005, -10064622992) },
            { "93s", (-23411875015, -20586133625, -10120806227) },
            { "J3o", (-20362606205, -18380628875, -10838401505) },
            { "65o", (-30941195020, -26722684230, -10919377676) },
            { "73s", (-29938913315, -25758844135, -10925826512) },
            { "T5o", (-22246444800, -19834634890, -11059635177) },
            { "92s", (-25454169860, -22272224630, -11073143137) },
            { "75o", (-29947810980, -25967444530, -11297850106) },
            { "52s", (-34179438165, -29082055315, -11586954907) },
            { "85o", (-28205588890, -24609311910, -11666024048) },
            { "83s", (-28794970995, -24966273815, -11699797436) },
            { "95o", (-25731642745, -22656242005, -11757380391) },
            { "T4o", (-23986263090, -21261142340, -11781052824) },
            { "J2o", (-22595017530, -20224117860, -11897768180) },
            { "82s", (-30212011765, -26130821545, -12348656783) },
            { "54o", (-35028432910, -30059404330, -12537284372) },
            { "42s", (-36592082155, -31066663115, -12565732392) },
            { "62s", (-35342119445, -30169071335, -12788333885) },
            { "T3o", (-26179440610, -23072747460, -12795516385) },
            { "64o", (-35641965540, -30612191750, -13238749633) },
            { "32s", (-38654181855, -32774447545, -13618875900) },
            { "72s", (-34972646915, -30003872725, -13641611576) },
            { "74o", (-34726753730, -29923240710, -13680444487) },
            { "T2o", (-28411472195, -24916224625, -13832529469) },
            { "84o", (-33023932740, -28596708950, -14064391301) },
            { "94o", (-30582288735, -26668660605, -14151855568) },
            { "53o", (-39656840850, -33894713450, -14857868499) },
            { "93o", (-32141939955, -27954981705, -14864777364) },
            { "63o", (-40378139770, -34537504360, -15630151320) },
            { "92o", (-34365866620, -29792293030, -15904024120) },
            { "43o", (-42306479590, -36074966780, -15942614647) },
            { "73o", (-39496065580, -33875518390, -16088934614) },
            { "83o", (-37821914720, -32571192870, -16463942411) },
            { "82o", (-39396897365, -33870915915, -17201367127) },
            { "52o", (-44497835675, -37905016905, -17269815450) },
            { "62o", (-45269576535, -38588866475, -18067722246) },
            { "42o", (-47063075085, -40015126995, -18302581952) },
            { "72o", (-44417749005, -37950365825, -18517505153) },
            { "32o", (-49253441875, -41829389825, -19420109898) }
        };
    }
}