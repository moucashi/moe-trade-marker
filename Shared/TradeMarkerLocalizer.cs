using System;
using System.Collections.Generic;
using System.Globalization;

namespace MoeTradeMarker.Shared;

public static class TradeMarkerLocalizer
{
    private static readonly Dictionary<TradeMarkerText, string> English = new()
    {
        [TradeMarkerText.ClientLoaded] = "Moe-TradeMarker client loaded.",
        [TradeMarkerText.ClientInitFailed] = "Moe-TradeMarker client initialization failed: {0}",
        [TradeMarkerText.ConfigGeneralSection] = "General",
        [TradeMarkerText.ConfigDisplaySection] = "Display",
        [TradeMarkerText.ConfigShowTraderMarkerDescription] = "Show the trader marker on items that have a trader marker.",
        [TradeMarkerText.ConfigMarkerPositionDescription] = "Trader marker icon position. Available values: LeftTop, RightTop, LeftBottom, RightBottom.",
        [TradeMarkerText.ConfigMarkerColorDescription] = "Trader marker icon color.",
        [TradeMarkerText.ConfigLanguageModeDescription] = "Language used by Moe-TradeMarker. Auto follows the detected game language.",
        [TradeMarkerText.TooltipTraderMarker] = "From trader {0}",
        [TradeMarkerText.ClientItemMarkerMissing] = "Moe-TradeMarker did not find a trader marker for item {0}.",
        [TradeMarkerText.ClientMarkerRefreshFailed] = "Moe-TradeMarker client marker data refresh failed: {0}",
        [TradeMarkerText.ClientItemViewTypeMissing] = "Moe-TradeMarker could not find EFT.UI.DragAndDrop.ItemView; item marker patch will not be installed.",
        [TradeMarkerText.ClientSetQuestItemViewPanelMissing] = "Moe-TradeMarker could not find ItemView.SetQuestItemViewPanel(); item marker patch will not be installed.",
        [TradeMarkerText.ClientItemViewPatchInstalled] = "Moe-TradeMarker installed the ItemView.SetQuestItemViewPanel item marker patch.",
        [TradeMarkerText.ClientLanguageSynced] = "Moe-TradeMarker synced client language to server: {0}.",
        [TradeMarkerText.ClientLanguageSyncFailed] = "Moe-TradeMarker client language sync failed: {0}",
        [TradeMarkerText.ClientItemReadFailed] = "Moe-TradeMarker could not read the item object from ItemView.",
        [TradeMarkerText.ClientMainImageReadFailed] = "Moe-TradeMarker could not read MainImage for item {0}.",
        [TradeMarkerText.ClientItemViewComponentReadFailed] = "Moe-TradeMarker could not convert ItemView for item {0} to a Unity component.",
        [TradeMarkerText.ServerLoaded] = "Moe-TradeMarker server loaded.",
        [TradeMarkerText.ServerConfigLoaded] = "Moe-TradeMarker config loaded.",
        [TradeMarkerText.ServerConfigLoadFailed] = "Moe-TradeMarker config load failed; default config is being used: {0}",
        [TradeMarkerText.RagfairMarkedItemBlocked] = "This item has a trader marker ({0}) and cannot be listed on the flea market.",
    };

    private static readonly Dictionary<TradeMarkerText, string> Chinese = new()
    {
        [TradeMarkerText.ClientLoaded] = "Moe-TradeMarker 客户端已加载。",
        [TradeMarkerText.ClientInitFailed] = "Moe-TradeMarker 客户端初始化失败：{0}",
        [TradeMarkerText.ConfigGeneralSection] = "通用",
        [TradeMarkerText.ConfigDisplaySection] = "显示",
        [TradeMarkerText.ConfigShowTraderMarkerDescription] = "是否在带有商人标记的物品图标上显示角标。",
        [TradeMarkerText.ConfigMarkerPositionDescription] = "商人标记角标位置，可选 LeftTop、RightTop、LeftBottom、RightBottom。",
        [TradeMarkerText.ConfigMarkerColorDescription] = "商人标记图标颜色。",
        [TradeMarkerText.ConfigLanguageModeDescription] = "Moe-TradeMarker 使用的语言。Auto 会跟随检测到的游戏语言。",
        [TradeMarkerText.TooltipTraderMarker] = "来自商人 {0}",
        [TradeMarkerText.ClientItemMarkerMissing] = "Moe-TradeMarker 未找到物品 {0} 的商人标记。",
        [TradeMarkerText.ClientMarkerRefreshFailed] = "Moe-TradeMarker 客户端标记数据刷新失败：{0}",
        [TradeMarkerText.ClientItemViewTypeMissing] = "Moe-TradeMarker 未找到 EFT.UI.DragAndDrop.ItemView，无法安装物品角标补丁。",
        [TradeMarkerText.ClientSetQuestItemViewPanelMissing] = "Moe-TradeMarker 未找到 ItemView.SetQuestItemViewPanel()，无法安装物品角标补丁。",
        [TradeMarkerText.ClientItemViewPatchInstalled] = "Moe-TradeMarker 已安装 ItemView.SetQuestItemViewPanel 物品角标补丁。",
        [TradeMarkerText.ClientLanguageSynced] = "Moe-TradeMarker 已将客户端语言同步到服务端：{0}。",
        [TradeMarkerText.ClientLanguageSyncFailed] = "Moe-TradeMarker 客户端语言同步失败：{0}",
        [TradeMarkerText.ClientItemReadFailed] = "Moe-TradeMarker 未能从 ItemView 读取物品对象。",
        [TradeMarkerText.ClientMainImageReadFailed] = "Moe-TradeMarker 未能从 ItemView 读取物品 {0} 的 MainImage。",
        [TradeMarkerText.ClientItemViewComponentReadFailed] = "Moe-TradeMarker 未能将物品 {0} 的 ItemView 转换为 Unity 组件。",
        [TradeMarkerText.ServerLoaded] = "Moe-TradeMarker 服务端已加载。",
        [TradeMarkerText.ServerConfigLoaded] = "Moe-TradeMarker 配置已加载。",
        [TradeMarkerText.ServerConfigLoadFailed] = "Moe-TradeMarker 配置读取失败，已使用默认配置：{0}",
        [TradeMarkerText.RagfairMarkedItemBlocked] = "物品带有商人标记（{0}），无法上架跳蚤市场。",
    };

    private static readonly Dictionary<TradeMarkerText, string> Czech = new()
    {
        [TradeMarkerText.ClientLoaded] = "Klient Moe-TradeMarker byl načten.",
        [TradeMarkerText.ClientInitFailed] = "Inicializace klienta Moe-TradeMarker selhala: {0}",
        [TradeMarkerText.ConfigGeneralSection] = "Obecné",
        [TradeMarkerText.ConfigDisplaySection] = "Zobrazení",
        [TradeMarkerText.ConfigShowTraderMarkerDescription] = "Zobrazit značku obchodníka u předmětů, které ji mají.",
        [TradeMarkerText.ConfigMarkerPositionDescription] = "Pozice ikony značky obchodníka. Dostupné hodnoty: LeftTop, RightTop, LeftBottom, RightBottom.",
        [TradeMarkerText.ConfigMarkerColorDescription] = "Barva ikony značky obchodníka.",
        [TradeMarkerText.ConfigLanguageModeDescription] = "Jazyk používaný Moe-TradeMarkerem. Auto následuje zjištěný jazyk hry.",
        [TradeMarkerText.TooltipTraderMarker] = "Od obchodníka {0}",
        [TradeMarkerText.ClientItemMarkerMissing] = "Moe-TradeMarker nenašel značku obchodníka pro předmět {0}.",
        [TradeMarkerText.ClientMarkerRefreshFailed] = "Obnovení dat značek klienta Moe-TradeMarker selhalo: {0}",
        [TradeMarkerText.ClientItemViewTypeMissing] = "Moe-TradeMarker nenašel EFT.UI.DragAndDrop.ItemView; patch značek předmětů nebude nainstalován.",
        [TradeMarkerText.ClientSetQuestItemViewPanelMissing] = "Moe-TradeMarker nenašel ItemView.SetQuestItemViewPanel(); patch značek předmětů nebude nainstalován.",
        [TradeMarkerText.ClientItemViewPatchInstalled] = "Moe-TradeMarker nainstaloval patch značek předmětů ItemView.SetQuestItemViewPanel.",
        [TradeMarkerText.ClientLanguageSynced] = "Moe-TradeMarker synchronizoval jazyk klienta se serverem: {0}.",
        [TradeMarkerText.ClientLanguageSyncFailed] = "Synchronizace jazyka klienta Moe-TradeMarker selhala: {0}",
        [TradeMarkerText.ClientItemReadFailed] = "Moe-TradeMarker nemohl načíst objekt předmětu z ItemView.",
        [TradeMarkerText.ClientMainImageReadFailed] = "Moe-TradeMarker nemohl načíst MainImage pro předmět {0}.",
        [TradeMarkerText.ClientItemViewComponentReadFailed] = "Moe-TradeMarker nemohl převést ItemView předmětu {0} na Unity komponentu.",
        [TradeMarkerText.ServerLoaded] = "Server Moe-TradeMarker byl načten.",
        [TradeMarkerText.ServerConfigLoaded] = "Konfigurace Moe-TradeMarker byla načtena.",
        [TradeMarkerText.ServerConfigLoadFailed] = "Načtení konfigurace Moe-TradeMarker selhalo; používá se výchozí konfigurace: {0}",
        [TradeMarkerText.RagfairMarkedItemBlocked] = "Tento předmět má značku obchodníka ({0}) a nelze jej vystavit na bleším trhu.",
    };

    private static readonly Dictionary<TradeMarkerText, string> French = new()
    {
        [TradeMarkerText.ClientLoaded] = "Client Moe-TradeMarker chargé.",
        [TradeMarkerText.ClientInitFailed] = "Échec de l'initialisation du client Moe-TradeMarker : {0}",
        [TradeMarkerText.ConfigGeneralSection] = "Général",
        [TradeMarkerText.ConfigDisplaySection] = "Affichage",
        [TradeMarkerText.ConfigShowTraderMarkerDescription] = "Afficher la marque de marchand sur les objets qui en possèdent une.",
        [TradeMarkerText.ConfigMarkerPositionDescription] = "Position de l'icône de marque de marchand. Valeurs disponibles : LeftTop, RightTop, LeftBottom, RightBottom.",
        [TradeMarkerText.ConfigMarkerColorDescription] = "Couleur de l'icône de marque de marchand.",
        [TradeMarkerText.ConfigLanguageModeDescription] = "Langue utilisée par Moe-TradeMarker. Auto suit la langue du jeu détectée.",
        [TradeMarkerText.TooltipTraderMarker] = "Depuis le marchand {0}",
        [TradeMarkerText.ClientItemMarkerMissing] = "Moe-TradeMarker n'a pas trouvé de marque de marchand pour l'objet {0}.",
        [TradeMarkerText.ClientMarkerRefreshFailed] = "Échec du rafraîchissement des données de marques du client Moe-TradeMarker : {0}",
        [TradeMarkerText.ClientItemViewTypeMissing] = "Moe-TradeMarker n'a pas trouvé EFT.UI.DragAndDrop.ItemView ; le patch de marque d'objet ne sera pas installé.",
        [TradeMarkerText.ClientSetQuestItemViewPanelMissing] = "Moe-TradeMarker n'a pas trouvé ItemView.SetQuestItemViewPanel() ; le patch de marque d'objet ne sera pas installé.",
        [TradeMarkerText.ClientItemViewPatchInstalled] = "Moe-TradeMarker a installé le patch de marque d'objet ItemView.SetQuestItemViewPanel.",
        [TradeMarkerText.ClientLanguageSynced] = "Moe-TradeMarker a synchronisé la langue du client avec le serveur : {0}.",
        [TradeMarkerText.ClientLanguageSyncFailed] = "Échec de la synchronisation de la langue du client Moe-TradeMarker : {0}",
        [TradeMarkerText.ClientItemReadFailed] = "Moe-TradeMarker n'a pas pu lire l'objet depuis ItemView.",
        [TradeMarkerText.ClientMainImageReadFailed] = "Moe-TradeMarker n'a pas pu lire MainImage pour l'objet {0}.",
        [TradeMarkerText.ClientItemViewComponentReadFailed] = "Moe-TradeMarker n'a pas pu convertir l'ItemView de l'objet {0} en composant Unity.",
        [TradeMarkerText.ServerLoaded] = "Serveur Moe-TradeMarker chargé.",
        [TradeMarkerText.ServerConfigLoaded] = "Configuration Moe-TradeMarker chargée.",
        [TradeMarkerText.ServerConfigLoadFailed] = "Échec du chargement de la configuration Moe-TradeMarker ; la configuration par défaut est utilisée : {0}",
        [TradeMarkerText.RagfairMarkedItemBlocked] = "Cet objet possède une marque de marchand ({0}) et ne peut pas être mis en vente au marché aux puces.",
    };

    private static readonly Dictionary<TradeMarkerText, string> German = new()
    {
        [TradeMarkerText.ClientLoaded] = "Moe-TradeMarker-Client geladen.",
        [TradeMarkerText.ClientInitFailed] = "Initialisierung des Moe-TradeMarker-Clients fehlgeschlagen: {0}",
        [TradeMarkerText.ConfigGeneralSection] = "Allgemein",
        [TradeMarkerText.ConfigDisplaySection] = "Anzeige",
        [TradeMarkerText.ConfigShowTraderMarkerDescription] = "Zeigt die Händlermarkierung auf Gegenständen an, die eine Markierung besitzen.",
        [TradeMarkerText.ConfigMarkerPositionDescription] = "Position der Händlermarkierung. Verfügbare Werte: LeftTop, RightTop, LeftBottom, RightBottom.",
        [TradeMarkerText.ConfigMarkerColorDescription] = "Farbe der Händlermarkierung.",
        [TradeMarkerText.ConfigLanguageModeDescription] = "Sprache von Moe-TradeMarker. Auto folgt der erkannten Spielsprache.",
        [TradeMarkerText.TooltipTraderMarker] = "Vom Händler {0}",
        [TradeMarkerText.ClientItemMarkerMissing] = "Moe-TradeMarker hat keine Händlermarkierung für Gegenstand {0} gefunden.",
        [TradeMarkerText.ClientMarkerRefreshFailed] = "Aktualisierung der Moe-TradeMarker-Clientdaten fehlgeschlagen: {0}",
        [TradeMarkerText.ClientItemViewTypeMissing] = "Moe-TradeMarker konnte EFT.UI.DragAndDrop.ItemView nicht finden; der Gegenstandsmarkierungs-Patch wird nicht installiert.",
        [TradeMarkerText.ClientSetQuestItemViewPanelMissing] = "Moe-TradeMarker konnte ItemView.SetQuestItemViewPanel() nicht finden; der Gegenstandsmarkierungs-Patch wird nicht installiert.",
        [TradeMarkerText.ClientItemViewPatchInstalled] = "Moe-TradeMarker hat den ItemView.SetQuestItemViewPanel-Patch für Gegenstandsmarkierungen installiert.",
        [TradeMarkerText.ClientLanguageSynced] = "Moe-TradeMarker hat die Clientsprache mit dem Server synchronisiert: {0}.",
        [TradeMarkerText.ClientLanguageSyncFailed] = "Synchronisierung der Moe-TradeMarker-Clientsprache fehlgeschlagen: {0}",
        [TradeMarkerText.ClientItemReadFailed] = "Moe-TradeMarker konnte das Gegenstandsobjekt aus ItemView nicht lesen.",
        [TradeMarkerText.ClientMainImageReadFailed] = "Moe-TradeMarker konnte MainImage für Gegenstand {0} nicht lesen.",
        [TradeMarkerText.ClientItemViewComponentReadFailed] = "Moe-TradeMarker konnte ItemView für Gegenstand {0} nicht in eine Unity-Komponente umwandeln.",
        [TradeMarkerText.ServerLoaded] = "Moe-TradeMarker-Server geladen.",
        [TradeMarkerText.ServerConfigLoaded] = "Moe-TradeMarker-Konfiguration geladen.",
        [TradeMarkerText.ServerConfigLoadFailed] = "Moe-TradeMarker-Konfiguration konnte nicht geladen werden; Standardkonfiguration wird verwendet: {0}",
        [TradeMarkerText.RagfairMarkedItemBlocked] = "Dieser Gegenstand besitzt eine Händlermarkierung ({0}) und kann nicht auf dem Flohmarkt angeboten werden.",
    };

    private static readonly Dictionary<TradeMarkerText, string> Hungarian = new()
    {
        [TradeMarkerText.ClientLoaded] = "A Moe-TradeMarker kliens betöltve.",
        [TradeMarkerText.ClientInitFailed] = "A Moe-TradeMarker kliens inicializálása sikertelen: {0}",
        [TradeMarkerText.ConfigGeneralSection] = "Általános",
        [TradeMarkerText.ConfigDisplaySection] = "Megjelenítés",
        [TradeMarkerText.ConfigShowTraderMarkerDescription] = "Kereskedőjelző megjelenítése azokon a tárgyakon, amelyek rendelkeznek vele.",
        [TradeMarkerText.ConfigMarkerPositionDescription] = "A kereskedőjelző ikon pozíciója. Elérhető értékek: LeftTop, RightTop, LeftBottom, RightBottom.",
        [TradeMarkerText.ConfigMarkerColorDescription] = "A kereskedőjelző ikon színe.",
        [TradeMarkerText.ConfigLanguageModeDescription] = "A Moe-TradeMarker által használt nyelv. Az Auto a felismert játéknyelvet követi.",
        [TradeMarkerText.TooltipTraderMarker] = "A kereskedőtől {0}",
        [TradeMarkerText.ClientItemMarkerMissing] = "A Moe-TradeMarker nem talált kereskedőjelzőt a(z) {0} tárgyhoz.",
        [TradeMarkerText.ClientMarkerRefreshFailed] = "A Moe-TradeMarker kliens jelzőadatainak frissítése sikertelen: {0}",
        [TradeMarkerText.ClientItemViewTypeMissing] = "A Moe-TradeMarker nem találta az EFT.UI.DragAndDrop.ItemView típust; a tárgyjelző patch nem lesz telepítve.",
        [TradeMarkerText.ClientSetQuestItemViewPanelMissing] = "A Moe-TradeMarker nem találta az ItemView.SetQuestItemViewPanel() metódust; a tárgyjelző patch nem lesz telepítve.",
        [TradeMarkerText.ClientItemViewPatchInstalled] = "A Moe-TradeMarker telepítette az ItemView.SetQuestItemViewPanel tárgyjelző patch-et.",
        [TradeMarkerText.ClientLanguageSynced] = "A Moe-TradeMarker szinkronizálta a kliens nyelvét a szerverrel: {0}.",
        [TradeMarkerText.ClientLanguageSyncFailed] = "A Moe-TradeMarker kliensnyelv szinkronizálása sikertelen: {0}",
        [TradeMarkerText.ClientItemReadFailed] = "A Moe-TradeMarker nem tudta kiolvasni a tárgy objektumot az ItemView-ból.",
        [TradeMarkerText.ClientMainImageReadFailed] = "A Moe-TradeMarker nem tudta kiolvasni a MainImage értéket a(z) {0} tárgyhoz.",
        [TradeMarkerText.ClientItemViewComponentReadFailed] = "A Moe-TradeMarker nem tudta a(z) {0} tárgy ItemView-ját Unity komponenssé alakítani.",
        [TradeMarkerText.ServerLoaded] = "A Moe-TradeMarker szerver betöltve.",
        [TradeMarkerText.ServerConfigLoaded] = "A Moe-TradeMarker konfiguráció betöltve.",
        [TradeMarkerText.ServerConfigLoadFailed] = "A Moe-TradeMarker konfiguráció betöltése sikertelen; az alapértelmezett konfiguráció lesz használva: {0}",
        [TradeMarkerText.RagfairMarkedItemBlocked] = "Ez a tárgy kereskedőjelzővel rendelkezik ({0}), ezért nem tehető fel a bolhapiacra.",
    };

    private static readonly Dictionary<TradeMarkerText, string> Italian = new()
    {
        [TradeMarkerText.ClientLoaded] = "Client Moe-TradeMarker caricato.",
        [TradeMarkerText.ClientInitFailed] = "Inizializzazione del client Moe-TradeMarker non riuscita: {0}",
        [TradeMarkerText.ConfigGeneralSection] = "Generale",
        [TradeMarkerText.ConfigDisplaySection] = "Visualizzazione",
        [TradeMarkerText.ConfigShowTraderMarkerDescription] = "Mostra il contrassegno del commerciante sugli oggetti che lo possiedono.",
        [TradeMarkerText.ConfigMarkerPositionDescription] = "Posizione dell'icona del contrassegno commerciante. Valori disponibili: LeftTop, RightTop, LeftBottom, RightBottom.",
        [TradeMarkerText.ConfigMarkerColorDescription] = "Colore dell'icona del contrassegno commerciante.",
        [TradeMarkerText.ConfigLanguageModeDescription] = "Lingua usata da Moe-TradeMarker. Auto segue la lingua di gioco rilevata.",
        [TradeMarkerText.TooltipTraderMarker] = "Dal commerciante {0}",
        [TradeMarkerText.ClientItemMarkerMissing] = "Moe-TradeMarker non ha trovato un contrassegno commerciante per l'oggetto {0}.",
        [TradeMarkerText.ClientMarkerRefreshFailed] = "Aggiornamento dei dati dei contrassegni del client Moe-TradeMarker non riuscito: {0}",
        [TradeMarkerText.ClientItemViewTypeMissing] = "Moe-TradeMarker non ha trovato EFT.UI.DragAndDrop.ItemView; la patch dei contrassegni oggetto non sarà installata.",
        [TradeMarkerText.ClientSetQuestItemViewPanelMissing] = "Moe-TradeMarker non ha trovato ItemView.SetQuestItemViewPanel(); la patch dei contrassegni oggetto non sarà installata.",
        [TradeMarkerText.ClientItemViewPatchInstalled] = "Moe-TradeMarker ha installato la patch dei contrassegni oggetto ItemView.SetQuestItemViewPanel.",
        [TradeMarkerText.ClientLanguageSynced] = "Moe-TradeMarker ha sincronizzato la lingua del client con il server: {0}.",
        [TradeMarkerText.ClientLanguageSyncFailed] = "Sincronizzazione della lingua del client Moe-TradeMarker non riuscita: {0}",
        [TradeMarkerText.ClientItemReadFailed] = "Moe-TradeMarker non ha potuto leggere l'oggetto da ItemView.",
        [TradeMarkerText.ClientMainImageReadFailed] = "Moe-TradeMarker non ha potuto leggere MainImage per l'oggetto {0}.",
        [TradeMarkerText.ClientItemViewComponentReadFailed] = "Moe-TradeMarker non ha potuto convertire l'ItemView dell'oggetto {0} in un componente Unity.",
        [TradeMarkerText.ServerLoaded] = "Server Moe-TradeMarker caricato.",
        [TradeMarkerText.ServerConfigLoaded] = "Configurazione Moe-TradeMarker caricata.",
        [TradeMarkerText.ServerConfigLoadFailed] = "Caricamento della configurazione Moe-TradeMarker non riuscito; verrà usata la configurazione predefinita: {0}",
        [TradeMarkerText.RagfairMarkedItemBlocked] = "Questo oggetto ha un contrassegno commerciante ({0}) e non può essere messo in vendita al mercatino.",
    };

    private static readonly Dictionary<TradeMarkerText, string> Japanese = new()
    {
        [TradeMarkerText.ClientLoaded] = "Moe-TradeMarker クライアントを読み込みました。",
        [TradeMarkerText.ClientInitFailed] = "Moe-TradeMarker クライアントの初期化に失敗しました: {0}",
        [TradeMarkerText.ConfigGeneralSection] = "一般",
        [TradeMarkerText.ConfigDisplaySection] = "表示",
        [TradeMarkerText.ConfigShowTraderMarkerDescription] = "商人マーカーを持つアイテムに商人マーカーを表示します。",
        [TradeMarkerText.ConfigMarkerPositionDescription] = "商人マーカーのアイコン位置。利用可能な値: LeftTop, RightTop, LeftBottom, RightBottom。",
        [TradeMarkerText.ConfigMarkerColorDescription] = "商人マーカーのアイコン色。",
        [TradeMarkerText.ConfigLanguageModeDescription] = "Moe-TradeMarker が使用する言語。Auto は検出されたゲーム言語に従います。",
        [TradeMarkerText.TooltipTraderMarker] = "商人 {0} から",
        [TradeMarkerText.ClientItemMarkerMissing] = "Moe-TradeMarker はアイテム {0} の商人マーカーを見つけられませんでした。",
        [TradeMarkerText.ClientMarkerRefreshFailed] = "Moe-TradeMarker クライアントのマーカーデータ更新に失敗しました: {0}",
        [TradeMarkerText.ClientItemViewTypeMissing] = "Moe-TradeMarker は EFT.UI.DragAndDrop.ItemView を見つけられませんでした。アイテムマーカーパッチはインストールされません。",
        [TradeMarkerText.ClientSetQuestItemViewPanelMissing] = "Moe-TradeMarker は ItemView.SetQuestItemViewPanel() を見つけられませんでした。アイテムマーカーパッチはインストールされません。",
        [TradeMarkerText.ClientItemViewPatchInstalled] = "Moe-TradeMarker は ItemView.SetQuestItemViewPanel アイテムマーカーパッチをインストールしました。",
        [TradeMarkerText.ClientLanguageSynced] = "Moe-TradeMarker はクライアント言語をサーバーへ同期しました: {0}。",
        [TradeMarkerText.ClientLanguageSyncFailed] = "Moe-TradeMarker クライアント言語の同期に失敗しました: {0}",
        [TradeMarkerText.ClientItemReadFailed] = "Moe-TradeMarker は ItemView からアイテムオブジェクトを読み取れませんでした。",
        [TradeMarkerText.ClientMainImageReadFailed] = "Moe-TradeMarker はアイテム {0} の MainImage を読み取れませんでした。",
        [TradeMarkerText.ClientItemViewComponentReadFailed] = "Moe-TradeMarker はアイテム {0} の ItemView を Unity コンポーネントへ変換できませんでした。",
        [TradeMarkerText.ServerLoaded] = "Moe-TradeMarker サーバーを読み込みました。",
        [TradeMarkerText.ServerConfigLoaded] = "Moe-TradeMarker 設定を読み込みました。",
        [TradeMarkerText.ServerConfigLoadFailed] = "Moe-TradeMarker 設定の読み込みに失敗しました。既定の設定を使用します: {0}",
        [TradeMarkerText.RagfairMarkedItemBlocked] = "このアイテムには商人マーカー ({0}) があるため、フリーマーケットに出品できません。",
    };

    private static readonly Dictionary<TradeMarkerText, string> Korean = new()
    {
        [TradeMarkerText.ClientLoaded] = "Moe-TradeMarker 클라이언트를 불러왔습니다.",
        [TradeMarkerText.ClientInitFailed] = "Moe-TradeMarker 클라이언트 초기화 실패: {0}",
        [TradeMarkerText.ConfigGeneralSection] = "일반",
        [TradeMarkerText.ConfigDisplaySection] = "표시",
        [TradeMarkerText.ConfigShowTraderMarkerDescription] = "상인 마커가 있는 아이템에 상인 마커를 표시합니다.",
        [TradeMarkerText.ConfigMarkerPositionDescription] = "상인 마커 아이콘 위치. 사용 가능한 값: LeftTop, RightTop, LeftBottom, RightBottom.",
        [TradeMarkerText.ConfigMarkerColorDescription] = "상인 마커 아이콘 색상.",
        [TradeMarkerText.ConfigLanguageModeDescription] = "Moe-TradeMarker에서 사용할 언어입니다. Auto는 감지된 게임 언어를 따릅니다.",
        [TradeMarkerText.TooltipTraderMarker] = "상인 {0}에게서",
        [TradeMarkerText.ClientItemMarkerMissing] = "Moe-TradeMarker가 아이템 {0}의 상인 마커를 찾지 못했습니다.",
        [TradeMarkerText.ClientMarkerRefreshFailed] = "Moe-TradeMarker 클라이언트 마커 데이터 새로 고침 실패: {0}",
        [TradeMarkerText.ClientItemViewTypeMissing] = "Moe-TradeMarker가 EFT.UI.DragAndDrop.ItemView를 찾지 못했습니다. 아이템 마커 패치가 설치되지 않습니다.",
        [TradeMarkerText.ClientSetQuestItemViewPanelMissing] = "Moe-TradeMarker가 ItemView.SetQuestItemViewPanel()을 찾지 못했습니다. 아이템 마커 패치가 설치되지 않습니다.",
        [TradeMarkerText.ClientItemViewPatchInstalled] = "Moe-TradeMarker가 ItemView.SetQuestItemViewPanel 아이템 마커 패치를 설치했습니다.",
        [TradeMarkerText.ClientLanguageSynced] = "Moe-TradeMarker가 클라이언트 언어를 서버와 동기화했습니다: {0}.",
        [TradeMarkerText.ClientLanguageSyncFailed] = "Moe-TradeMarker 클라이언트 언어 동기화 실패: {0}",
        [TradeMarkerText.ClientItemReadFailed] = "Moe-TradeMarker가 ItemView에서 아이템 객체를 읽지 못했습니다.",
        [TradeMarkerText.ClientMainImageReadFailed] = "Moe-TradeMarker가 아이템 {0}의 MainImage를 읽지 못했습니다.",
        [TradeMarkerText.ClientItemViewComponentReadFailed] = "Moe-TradeMarker가 아이템 {0}의 ItemView를 Unity 컴포넌트로 변환하지 못했습니다.",
        [TradeMarkerText.ServerLoaded] = "Moe-TradeMarker 서버를 불러왔습니다.",
        [TradeMarkerText.ServerConfigLoaded] = "Moe-TradeMarker 설정을 불러왔습니다.",
        [TradeMarkerText.ServerConfigLoadFailed] = "Moe-TradeMarker 설정을 불러오지 못했습니다. 기본 설정을 사용합니다: {0}",
        [TradeMarkerText.RagfairMarkedItemBlocked] = "이 아이템에는 상인 마커({0})가 있어 플리 마켓에 등록할 수 없습니다.",
    };

    private static readonly Dictionary<TradeMarkerText, string> Polish = new()
    {
        [TradeMarkerText.ClientLoaded] = "Klient Moe-TradeMarker załadowany.",
        [TradeMarkerText.ClientInitFailed] = "Inicjalizacja klienta Moe-TradeMarker nie powiodła się: {0}",
        [TradeMarkerText.ConfigGeneralSection] = "Ogólne",
        [TradeMarkerText.ConfigDisplaySection] = "Wyświetlanie",
        [TradeMarkerText.ConfigShowTraderMarkerDescription] = "Pokaż znacznik handlarza na przedmiotach, które go mają.",
        [TradeMarkerText.ConfigMarkerPositionDescription] = "Pozycja ikony znacznika handlarza. Dostępne wartości: LeftTop, RightTop, LeftBottom, RightBottom.",
        [TradeMarkerText.ConfigMarkerColorDescription] = "Kolor ikony znacznika handlarza.",
        [TradeMarkerText.ConfigLanguageModeDescription] = "Język używany przez Moe-TradeMarker. Auto podąża za wykrytym językiem gry.",
        [TradeMarkerText.TooltipTraderMarker] = "Od handlarza {0}",
        [TradeMarkerText.ClientItemMarkerMissing] = "Moe-TradeMarker nie znalazł znacznika handlarza dla przedmiotu {0}.",
        [TradeMarkerText.ClientMarkerRefreshFailed] = "Odświeżenie danych znaczników klienta Moe-TradeMarker nie powiodło się: {0}",
        [TradeMarkerText.ClientItemViewTypeMissing] = "Moe-TradeMarker nie mógł znaleźć EFT.UI.DragAndDrop.ItemView; patch znacznika przedmiotów nie zostanie zainstalowany.",
        [TradeMarkerText.ClientSetQuestItemViewPanelMissing] = "Moe-TradeMarker nie mógł znaleźć ItemView.SetQuestItemViewPanel(); patch znacznika przedmiotów nie zostanie zainstalowany.",
        [TradeMarkerText.ClientItemViewPatchInstalled] = "Moe-TradeMarker zainstalował patch znacznika przedmiotów ItemView.SetQuestItemViewPanel.",
        [TradeMarkerText.ClientLanguageSynced] = "Moe-TradeMarker zsynchronizował język klienta z serwerem: {0}.",
        [TradeMarkerText.ClientLanguageSyncFailed] = "Synchronizacja języka klienta Moe-TradeMarker nie powiodła się: {0}",
        [TradeMarkerText.ClientItemReadFailed] = "Moe-TradeMarker nie mógł odczytać obiektu przedmiotu z ItemView.",
        [TradeMarkerText.ClientMainImageReadFailed] = "Moe-TradeMarker nie mógł odczytać MainImage dla przedmiotu {0}.",
        [TradeMarkerText.ClientItemViewComponentReadFailed] = "Moe-TradeMarker nie mógł przekonwertować ItemView przedmiotu {0} na komponent Unity.",
        [TradeMarkerText.ServerLoaded] = "Serwer Moe-TradeMarker załadowany.",
        [TradeMarkerText.ServerConfigLoaded] = "Konfiguracja Moe-TradeMarker załadowana.",
        [TradeMarkerText.ServerConfigLoadFailed] = "Nie udało się załadować konfiguracji Moe-TradeMarker; używana jest konfiguracja domyślna: {0}",
        [TradeMarkerText.RagfairMarkedItemBlocked] = "Ten przedmiot ma znacznik handlarza ({0}) i nie może zostać wystawiony na pchlim targu.",
    };

    private static readonly Dictionary<TradeMarkerText, string> Portuguese = new()
    {
        [TradeMarkerText.ClientLoaded] = "Cliente Moe-TradeMarker carregado.",
        [TradeMarkerText.ClientInitFailed] = "Falha ao inicializar o cliente Moe-TradeMarker: {0}",
        [TradeMarkerText.ConfigGeneralSection] = "Geral",
        [TradeMarkerText.ConfigDisplaySection] = "Visualização",
        [TradeMarkerText.ConfigShowTraderMarkerDescription] = "Mostrar a marca do comerciante em itens que tenham uma marca de comerciante.",
        [TradeMarkerText.ConfigMarkerPositionDescription] = "Posição do ícone da marca do comerciante. Valores disponíveis: LeftTop, RightTop, LeftBottom, RightBottom.",
        [TradeMarkerText.ConfigMarkerColorDescription] = "Cor do ícone da marca do comerciante.",
        [TradeMarkerText.ConfigLanguageModeDescription] = "Idioma usado pelo Moe-TradeMarker. Auto segue o idioma do jogo detectado.",
        [TradeMarkerText.TooltipTraderMarker] = "Do comerciante {0}",
        [TradeMarkerText.ClientItemMarkerMissing] = "Moe-TradeMarker não encontrou uma marca de comerciante para o item {0}.",
        [TradeMarkerText.ClientMarkerRefreshFailed] = "Falha ao atualizar os dados de marcas do cliente Moe-TradeMarker: {0}",
        [TradeMarkerText.ClientItemViewTypeMissing] = "Moe-TradeMarker não encontrou EFT.UI.DragAndDrop.ItemView; o patch de marca de item não será instalado.",
        [TradeMarkerText.ClientSetQuestItemViewPanelMissing] = "Moe-TradeMarker não encontrou ItemView.SetQuestItemViewPanel(); o patch de marca de item não será instalado.",
        [TradeMarkerText.ClientItemViewPatchInstalled] = "Moe-TradeMarker instalou o patch de marca de item ItemView.SetQuestItemViewPanel.",
        [TradeMarkerText.ClientLanguageSynced] = "Moe-TradeMarker sincronizou o idioma do cliente com o servidor: {0}.",
        [TradeMarkerText.ClientLanguageSyncFailed] = "Falha ao sincronizar o idioma do cliente Moe-TradeMarker: {0}",
        [TradeMarkerText.ClientItemReadFailed] = "Moe-TradeMarker não conseguiu ler o objeto do item a partir de ItemView.",
        [TradeMarkerText.ClientMainImageReadFailed] = "Moe-TradeMarker não conseguiu ler MainImage para o item {0}.",
        [TradeMarkerText.ClientItemViewComponentReadFailed] = "Moe-TradeMarker não conseguiu converter o ItemView do item {0} para um componente Unity.",
        [TradeMarkerText.ServerLoaded] = "Servidor Moe-TradeMarker carregado.",
        [TradeMarkerText.ServerConfigLoaded] = "Configuração do Moe-TradeMarker carregada.",
        [TradeMarkerText.ServerConfigLoadFailed] = "Falha ao carregar a configuração do Moe-TradeMarker; a configuração padrão será usada: {0}",
        [TradeMarkerText.RagfairMarkedItemBlocked] = "Este item tem uma marca de comerciante ({0}) e não pode ser listado no mercado de pulgas.",
    };

    private static readonly Dictionary<TradeMarkerText, string> Slovak = new()
    {
        [TradeMarkerText.ClientLoaded] = "Klient Moe-TradeMarker bol načítaný.",
        [TradeMarkerText.ClientInitFailed] = "Inicializácia klienta Moe-TradeMarker zlyhala: {0}",
        [TradeMarkerText.ConfigGeneralSection] = "Všeobecné",
        [TradeMarkerText.ConfigDisplaySection] = "Zobrazenie",
        [TradeMarkerText.ConfigShowTraderMarkerDescription] = "Zobraziť značku obchodníka na predmetoch, ktoré ju majú.",
        [TradeMarkerText.ConfigMarkerPositionDescription] = "Pozícia ikony značky obchodníka. Dostupné hodnoty: LeftTop, RightTop, LeftBottom, RightBottom.",
        [TradeMarkerText.ConfigMarkerColorDescription] = "Farba ikony značky obchodníka.",
        [TradeMarkerText.ConfigLanguageModeDescription] = "Jazyk používaný Moe-TradeMarkerom. Auto nasleduje zistený jazyk hry.",
        [TradeMarkerText.TooltipTraderMarker] = "Od obchodníka {0}",
        [TradeMarkerText.ClientItemMarkerMissing] = "Moe-TradeMarker nenašiel značku obchodníka pre predmet {0}.",
        [TradeMarkerText.ClientMarkerRefreshFailed] = "Obnovenie dát značiek klienta Moe-TradeMarker zlyhalo: {0}",
        [TradeMarkerText.ClientItemViewTypeMissing] = "Moe-TradeMarker nenašiel EFT.UI.DragAndDrop.ItemView; patch značiek predmetov nebude nainštalovaný.",
        [TradeMarkerText.ClientSetQuestItemViewPanelMissing] = "Moe-TradeMarker nenašiel ItemView.SetQuestItemViewPanel(); patch značiek predmetov nebude nainštalovaný.",
        [TradeMarkerText.ClientItemViewPatchInstalled] = "Moe-TradeMarker nainštaloval patch značiek predmetov ItemView.SetQuestItemViewPanel.",
        [TradeMarkerText.ClientLanguageSynced] = "Moe-TradeMarker synchronizoval jazyk klienta so serverom: {0}.",
        [TradeMarkerText.ClientLanguageSyncFailed] = "Synchronizácia jazyka klienta Moe-TradeMarker zlyhala: {0}",
        [TradeMarkerText.ClientItemReadFailed] = "Moe-TradeMarker nedokázal prečítať objekt predmetu z ItemView.",
        [TradeMarkerText.ClientMainImageReadFailed] = "Moe-TradeMarker nedokázal prečítať MainImage pre predmet {0}.",
        [TradeMarkerText.ClientItemViewComponentReadFailed] = "Moe-TradeMarker nedokázal previesť ItemView predmetu {0} na Unity komponent.",
        [TradeMarkerText.ServerLoaded] = "Server Moe-TradeMarker bol načítaný.",
        [TradeMarkerText.ServerConfigLoaded] = "Konfigurácia Moe-TradeMarker bola načítaná.",
        [TradeMarkerText.ServerConfigLoadFailed] = "Načítanie konfigurácie Moe-TradeMarker zlyhalo; používa sa predvolená konfigurácia: {0}",
        [TradeMarkerText.RagfairMarkedItemBlocked] = "Tento predmet má značku obchodníka ({0}) a nemožno ho vystaviť na blšom trhu.",
    };

    private static readonly Dictionary<TradeMarkerText, string> Spanish = new()
    {
        [TradeMarkerText.ClientLoaded] = "Cliente Moe-TradeMarker cargado.",
        [TradeMarkerText.ClientInitFailed] = "Error al inicializar el cliente Moe-TradeMarker: {0}",
        [TradeMarkerText.ConfigGeneralSection] = "General",
        [TradeMarkerText.ConfigDisplaySection] = "Visualización",
        [TradeMarkerText.ConfigShowTraderMarkerDescription] = "Muestra la marca de comerciante en los objetos que la tengan.",
        [TradeMarkerText.ConfigMarkerPositionDescription] = "Posición del icono de la marca de comerciante. Valores disponibles: LeftTop, RightTop, LeftBottom, RightBottom.",
        [TradeMarkerText.ConfigMarkerColorDescription] = "Color del icono de la marca de comerciante.",
        [TradeMarkerText.ConfigLanguageModeDescription] = "Idioma usado por Moe-TradeMarker. Auto sigue el idioma del juego detectado.",
        [TradeMarkerText.TooltipTraderMarker] = "Del comerciante {0}",
        [TradeMarkerText.ClientItemMarkerMissing] = "Moe-TradeMarker no encontró una marca de comerciante para el objeto {0}.",
        [TradeMarkerText.ClientMarkerRefreshFailed] = "Error al actualizar los datos de marcas del cliente Moe-TradeMarker: {0}",
        [TradeMarkerText.ClientItemViewTypeMissing] = "Moe-TradeMarker no pudo encontrar EFT.UI.DragAndDrop.ItemView; no se instalará el parche de marcas de objetos.",
        [TradeMarkerText.ClientSetQuestItemViewPanelMissing] = "Moe-TradeMarker no pudo encontrar ItemView.SetQuestItemViewPanel(); no se instalará el parche de marcas de objetos.",
        [TradeMarkerText.ClientItemViewPatchInstalled] = "Moe-TradeMarker instaló el parche de marcas de objetos ItemView.SetQuestItemViewPanel.",
        [TradeMarkerText.ClientLanguageSynced] = "Moe-TradeMarker sincronizó el idioma del cliente con el servidor: {0}.",
        [TradeMarkerText.ClientLanguageSyncFailed] = "Error al sincronizar el idioma del cliente Moe-TradeMarker: {0}",
        [TradeMarkerText.ClientItemReadFailed] = "Moe-TradeMarker no pudo leer el objeto desde ItemView.",
        [TradeMarkerText.ClientMainImageReadFailed] = "Moe-TradeMarker no pudo leer MainImage para el objeto {0}.",
        [TradeMarkerText.ClientItemViewComponentReadFailed] = "Moe-TradeMarker no pudo convertir el ItemView del objeto {0} en un componente de Unity.",
        [TradeMarkerText.ServerLoaded] = "Servidor Moe-TradeMarker cargado.",
        [TradeMarkerText.ServerConfigLoaded] = "Configuración de Moe-TradeMarker cargada.",
        [TradeMarkerText.ServerConfigLoadFailed] = "Error al cargar la configuración de Moe-TradeMarker; se usará la configuración predeterminada: {0}",
        [TradeMarkerText.RagfairMarkedItemBlocked] = "Este objeto tiene una marca de comerciante ({0}) y no se puede publicar en el mercadillo.",
    };

    private static readonly Dictionary<TradeMarkerText, string> Turkish = new()
    {
        [TradeMarkerText.ClientLoaded] = "Moe-TradeMarker istemcisi yüklendi.",
        [TradeMarkerText.ClientInitFailed] = "Moe-TradeMarker istemcisi başlatılamadı: {0}",
        [TradeMarkerText.ConfigGeneralSection] = "Genel",
        [TradeMarkerText.ConfigDisplaySection] = "Görüntü",
        [TradeMarkerText.ConfigShowTraderMarkerDescription] = "Tüccar işareti olan eşyalarda tüccar işaretini gösterir.",
        [TradeMarkerText.ConfigMarkerPositionDescription] = "Tüccar işareti simge konumu. Kullanılabilir değerler: LeftTop, RightTop, LeftBottom, RightBottom.",
        [TradeMarkerText.ConfigMarkerColorDescription] = "Tüccar işareti simge rengi.",
        [TradeMarkerText.ConfigLanguageModeDescription] = "Moe-TradeMarker tarafından kullanılan dil. Auto algılanan oyun dilini takip eder.",
        [TradeMarkerText.TooltipTraderMarker] = "{0} tüccarından",
        [TradeMarkerText.ClientItemMarkerMissing] = "Moe-TradeMarker, {0} eşyası için tüccar işareti bulamadı.",
        [TradeMarkerText.ClientMarkerRefreshFailed] = "Moe-TradeMarker istemci işaret verileri yenilenemedi: {0}",
        [TradeMarkerText.ClientItemViewTypeMissing] = "Moe-TradeMarker EFT.UI.DragAndDrop.ItemView bulamadı; eşya işareti yaması kurulmayacak.",
        [TradeMarkerText.ClientSetQuestItemViewPanelMissing] = "Moe-TradeMarker ItemView.SetQuestItemViewPanel() bulamadı; eşya işareti yaması kurulmayacak.",
        [TradeMarkerText.ClientItemViewPatchInstalled] = "Moe-TradeMarker ItemView.SetQuestItemViewPanel eşya işareti yamasını kurdu.",
        [TradeMarkerText.ClientLanguageSynced] = "Moe-TradeMarker istemci dilini sunucuyla eşitledi: {0}.",
        [TradeMarkerText.ClientLanguageSyncFailed] = "Moe-TradeMarker istemci dili eşitlenemedi: {0}",
        [TradeMarkerText.ClientItemReadFailed] = "Moe-TradeMarker ItemView üzerinden eşya nesnesini okuyamadı.",
        [TradeMarkerText.ClientMainImageReadFailed] = "Moe-TradeMarker {0} eşyası için MainImage okuyamadı.",
        [TradeMarkerText.ClientItemViewComponentReadFailed] = "Moe-TradeMarker {0} eşyasının ItemView değerini Unity bileşenine dönüştüremedi.",
        [TradeMarkerText.ServerLoaded] = "Moe-TradeMarker sunucusu yüklendi.",
        [TradeMarkerText.ServerConfigLoaded] = "Moe-TradeMarker yapılandırması yüklendi.",
        [TradeMarkerText.ServerConfigLoadFailed] = "Moe-TradeMarker yapılandırması yüklenemedi; varsayılan yapılandırma kullanılıyor: {0}",
        [TradeMarkerText.RagfairMarkedItemBlocked] = "Bu eşyada tüccar işareti ({0}) var ve bit pazarına konulamaz.",
    };

    private static readonly Dictionary<TradeMarkerText, string> Russian = new()
    {
        [TradeMarkerText.ClientLoaded] = "Клиент Moe-TradeMarker загружен.",
        [TradeMarkerText.ClientInitFailed] = "Не удалось инициализировать клиент Moe-TradeMarker: {0}",
        [TradeMarkerText.ConfigGeneralSection] = "Общие",
        [TradeMarkerText.ConfigDisplaySection] = "Отображение",
        [TradeMarkerText.ConfigShowTraderMarkerDescription] = "Показывать метку торговца на предметах, у которых она есть.",
        [TradeMarkerText.ConfigMarkerPositionDescription] = "Положение значка метки торговца. Доступные значения: LeftTop, RightTop, LeftBottom, RightBottom.",
        [TradeMarkerText.ConfigMarkerColorDescription] = "Цвет значка метки торговца.",
        [TradeMarkerText.ConfigLanguageModeDescription] = "Язык Moe-TradeMarker. Auto использует обнаруженный язык игры.",
        [TradeMarkerText.TooltipTraderMarker] = "От торговца {0}",
        [TradeMarkerText.ClientItemMarkerMissing] = "Moe-TradeMarker не нашел метку торговца для предмета {0}.",
        [TradeMarkerText.ClientMarkerRefreshFailed] = "Не удалось обновить данные меток клиента Moe-TradeMarker: {0}",
        [TradeMarkerText.ClientItemViewTypeMissing] = "Moe-TradeMarker не смог найти EFT.UI.DragAndDrop.ItemView; патч меток предметов не будет установлен.",
        [TradeMarkerText.ClientSetQuestItemViewPanelMissing] = "Moe-TradeMarker не смог найти ItemView.SetQuestItemViewPanel(); патч меток предметов не будет установлен.",
        [TradeMarkerText.ClientItemViewPatchInstalled] = "Moe-TradeMarker установил патч меток предметов ItemView.SetQuestItemViewPanel.",
        [TradeMarkerText.ClientLanguageSynced] = "Moe-TradeMarker синхронизировал язык клиента с сервером: {0}.",
        [TradeMarkerText.ClientLanguageSyncFailed] = "Не удалось синхронизировать язык клиента Moe-TradeMarker: {0}",
        [TradeMarkerText.ClientItemReadFailed] = "Moe-TradeMarker не смог прочитать объект предмета из ItemView.",
        [TradeMarkerText.ClientMainImageReadFailed] = "Moe-TradeMarker не смог прочитать MainImage для предмета {0}.",
        [TradeMarkerText.ClientItemViewComponentReadFailed] = "Moe-TradeMarker не смог преобразовать ItemView предмета {0} в компонент Unity.",
        [TradeMarkerText.ServerLoaded] = "Сервер Moe-TradeMarker загружен.",
        [TradeMarkerText.ServerConfigLoaded] = "Конфигурация Moe-TradeMarker загружена.",
        [TradeMarkerText.ServerConfigLoadFailed] = "Не удалось загрузить конфигурацию Moe-TradeMarker; используется конфигурация по умолчанию: {0}",
        [TradeMarkerText.RagfairMarkedItemBlocked] = "У этого предмета есть метка торговца ({0}), поэтому его нельзя выставить на барахолку.",
    };

    private static readonly Dictionary<TradeMarkerText, string> Romanian = new()
    {
        [TradeMarkerText.ClientLoaded] = "Clientul Moe-TradeMarker a fost încărcat.",
        [TradeMarkerText.ClientInitFailed] = "Inițializarea clientului Moe-TradeMarker a eșuat: {0}",
        [TradeMarkerText.ConfigGeneralSection] = "General",
        [TradeMarkerText.ConfigDisplaySection] = "Afișare",
        [TradeMarkerText.ConfigShowTraderMarkerDescription] = "Afișează marcajul comerciantului pe obiectele care au un astfel de marcaj.",
        [TradeMarkerText.ConfigMarkerPositionDescription] = "Poziția pictogramei marcajului comerciantului. Valori disponibile: LeftTop, RightTop, LeftBottom, RightBottom.",
        [TradeMarkerText.ConfigMarkerColorDescription] = "Culoarea pictogramei marcajului comerciantului.",
        [TradeMarkerText.ConfigLanguageModeDescription] = "Limba folosită de Moe-TradeMarker. Auto urmează limba jocului detectată.",
        [TradeMarkerText.TooltipTraderMarker] = "De la comerciantul {0}",
        [TradeMarkerText.ClientItemMarkerMissing] = "Moe-TradeMarker nu a găsit un marcaj de comerciant pentru obiectul {0}.",
        [TradeMarkerText.ClientMarkerRefreshFailed] = "Reîmprospătarea datelor de marcaj ale clientului Moe-TradeMarker a eșuat: {0}",
        [TradeMarkerText.ClientItemViewTypeMissing] = "Moe-TradeMarker nu a putut găsi EFT.UI.DragAndDrop.ItemView; patch-ul de marcaj al obiectelor nu va fi instalat.",
        [TradeMarkerText.ClientSetQuestItemViewPanelMissing] = "Moe-TradeMarker nu a putut găsi ItemView.SetQuestItemViewPanel(); patch-ul de marcaj al obiectelor nu va fi instalat.",
        [TradeMarkerText.ClientItemViewPatchInstalled] = "Moe-TradeMarker a instalat patch-ul de marcaj al obiectelor ItemView.SetQuestItemViewPanel.",
        [TradeMarkerText.ClientLanguageSynced] = "Moe-TradeMarker a sincronizat limba clientului cu serverul: {0}.",
        [TradeMarkerText.ClientLanguageSyncFailed] = "Sincronizarea limbii clientului Moe-TradeMarker a eșuat: {0}",
        [TradeMarkerText.ClientItemReadFailed] = "Moe-TradeMarker nu a putut citi obiectul din ItemView.",
        [TradeMarkerText.ClientMainImageReadFailed] = "Moe-TradeMarker nu a putut citi MainImage pentru obiectul {0}.",
        [TradeMarkerText.ClientItemViewComponentReadFailed] = "Moe-TradeMarker nu a putut converti ItemView pentru obiectul {0} într-o componentă Unity.",
        [TradeMarkerText.ServerLoaded] = "Serverul Moe-TradeMarker a fost încărcat.",
        [TradeMarkerText.ServerConfigLoaded] = "Configurația Moe-TradeMarker a fost încărcată.",
        [TradeMarkerText.ServerConfigLoadFailed] = "Încărcarea configurației Moe-TradeMarker a eșuat; se folosește configurația implicită: {0}",
        [TradeMarkerText.RagfairMarkedItemBlocked] = "Acest obiect are un marcaj de comerciant ({0}) și nu poate fi listat pe piața de vechituri.",
    };

    private static readonly Dictionary<TradeMarkerLanguage, IReadOnlyDictionary<TradeMarkerText, string>> Tables = new()
    {
        [TradeMarkerLanguage.English] = English,
        [TradeMarkerLanguage.Chinese] = Chinese,
        [TradeMarkerLanguage.Czech] = Czech,
        [TradeMarkerLanguage.French] = French,
        [TradeMarkerLanguage.German] = German,
        [TradeMarkerLanguage.Hungarian] = Hungarian,
        [TradeMarkerLanguage.Italian] = Italian,
        [TradeMarkerLanguage.Japanese] = Japanese,
        [TradeMarkerLanguage.Korean] = Korean,
        [TradeMarkerLanguage.Polish] = Polish,
        [TradeMarkerLanguage.Portuguese] = Portuguese,
        [TradeMarkerLanguage.Slovak] = Slovak,
        [TradeMarkerLanguage.Spanish] = Spanish,
        [TradeMarkerLanguage.SpanishMexico] = Spanish,
        [TradeMarkerLanguage.Turkish] = Turkish,
        [TradeMarkerLanguage.Russian] = Russian,
        [TradeMarkerLanguage.Romanian] = Romanian,
    };

    public static TradeMarkerLanguage DetectLanguage(string? language)
    {
        return TryDetectLanguage(language, out var detected) ? detected : TradeMarkerLanguage.English;
    }

    public static bool IsSupportedLanguageCode(object? language)
    {
        return TryDetectLanguage(language?.ToString(), out _);
    }

    public static string GetLanguageCode(TradeMarkerLanguage language)
    {
        return language switch
        {
            TradeMarkerLanguage.Chinese => "ch",
            TradeMarkerLanguage.Czech => "cz",
            TradeMarkerLanguage.French => "fr",
            TradeMarkerLanguage.German => "ge",
            TradeMarkerLanguage.Hungarian => "hu",
            TradeMarkerLanguage.Italian => "it",
            TradeMarkerLanguage.Japanese => "jp",
            TradeMarkerLanguage.Korean => "kr",
            TradeMarkerLanguage.Polish => "pl",
            TradeMarkerLanguage.Portuguese => "po",
            TradeMarkerLanguage.Slovak => "sk",
            TradeMarkerLanguage.Spanish => "es",
            TradeMarkerLanguage.SpanishMexico => "es-mx",
            TradeMarkerLanguage.Turkish => "tu",
            TradeMarkerLanguage.Russian => "ru",
            TradeMarkerLanguage.Romanian => "ro",
            _ => "en",
        };
    }

    public static string Text(TradeMarkerText key, TradeMarkerLanguage language)
    {
        if (Tables.TryGetValue(language, out var table) && table.TryGetValue(key, out var value))
        {
            return value;
        }

        return English.TryGetValue(key, out var fallback) ? fallback : key.ToString();
    }

    public static string Format(TradeMarkerText key, TradeMarkerLanguage language, params object?[] args)
    {
        return string.Format(CultureInfo.InvariantCulture, Text(key, language), args);
    }

    private static bool TryDetectLanguage(string? language, out TradeMarkerLanguage detected)
    {
        detected = TradeMarkerLanguage.English;
        if (string.IsNullOrWhiteSpace(language))
        {
            return false;
        }

        var normalized = NormalizeLanguageCode(language!);
        switch (normalized)
        {
            case "en":
            case "eng":
            case "english":
                detected = TradeMarkerLanguage.English;
                return true;
            case "ch":
            case "chs":
            case "cht":
            case "cn":
            case "zh":
            case "zh-cn":
            case "zh-hans":
            case "zh-hant":
            case "zh-tw":
            case "chinese":
            case "chinesesimplified":
            case "chinesetraditional":
                detected = TradeMarkerLanguage.Chinese;
                return true;
            case "cz":
            case "cs":
            case "cze":
            case "czech":
                detected = TradeMarkerLanguage.Czech;
                return true;
            case "fr":
            case "fra":
            case "fre":
            case "french":
                detected = TradeMarkerLanguage.French;
                return true;
            case "ge":
            case "de":
            case "deu":
            case "ger":
            case "german":
                detected = TradeMarkerLanguage.German;
                return true;
            case "hu":
            case "hun":
            case "hungarian":
                detected = TradeMarkerLanguage.Hungarian;
                return true;
            case "it":
            case "ita":
            case "italian":
                detected = TradeMarkerLanguage.Italian;
                return true;
            case "jp":
            case "ja":
            case "jpn":
            case "japanese":
                detected = TradeMarkerLanguage.Japanese;
                return true;
            case "kr":
            case "ko":
            case "kor":
            case "korean":
                detected = TradeMarkerLanguage.Korean;
                return true;
            case "pl":
            case "pol":
            case "polish":
                detected = TradeMarkerLanguage.Polish;
                return true;
            case "po":
            case "pt":
            case "pt-br":
            case "pt-pt":
            case "por":
            case "portugal":
            case "portuguese":
                detected = TradeMarkerLanguage.Portuguese;
                return true;
            case "sk":
            case "slo":
            case "slk":
            case "slovak":
                detected = TradeMarkerLanguage.Slovak;
                return true;
            case "es-mx":
            case "es-mex":
            case "spanishmexico":
                detected = TradeMarkerLanguage.SpanishMexico;
                return true;
            case "es":
            case "es-es":
            case "spa":
            case "spanish":
                detected = TradeMarkerLanguage.Spanish;
                return true;
            case "tu":
            case "tr":
            case "tur":
            case "turkish":
                detected = TradeMarkerLanguage.Turkish;
                return true;
            case "ru":
            case "rus":
            case "russian":
                detected = TradeMarkerLanguage.Russian;
                return true;
            case "ro":
            case "ron":
            case "rum":
            case "romanian":
                detected = TradeMarkerLanguage.Romanian;
                return true;
        }

        if (normalized.StartsWith("en-", StringComparison.Ordinal))
        {
            detected = TradeMarkerLanguage.English;
            return true;
        }

        if (normalized.StartsWith("zh-", StringComparison.Ordinal)
            || normalized.StartsWith("cn-", StringComparison.Ordinal)
            || normalized.StartsWith("ch-", StringComparison.Ordinal)
            || normalized.StartsWith("chinese", StringComparison.Ordinal))
        {
            detected = TradeMarkerLanguage.Chinese;
            return true;
        }

        if (normalized.StartsWith("fr-", StringComparison.Ordinal))
        {
            detected = TradeMarkerLanguage.French;
            return true;
        }

        if (normalized.StartsWith("de-", StringComparison.Ordinal)
            || normalized.StartsWith("ge-", StringComparison.Ordinal))
        {
            detected = TradeMarkerLanguage.German;
            return true;
        }

        if (normalized.StartsWith("ja-", StringComparison.Ordinal)
            || normalized.StartsWith("jp-", StringComparison.Ordinal))
        {
            detected = TradeMarkerLanguage.Japanese;
            return true;
        }

        if (normalized.StartsWith("ko-", StringComparison.Ordinal)
            || normalized.StartsWith("kr-", StringComparison.Ordinal))
        {
            detected = TradeMarkerLanguage.Korean;
            return true;
        }

        if (normalized.StartsWith("pt-", StringComparison.Ordinal)
            || normalized.StartsWith("po-", StringComparison.Ordinal))
        {
            detected = TradeMarkerLanguage.Portuguese;
            return true;
        }

        if (normalized.StartsWith("es-mx", StringComparison.Ordinal))
        {
            detected = TradeMarkerLanguage.SpanishMexico;
            return true;
        }

        if (normalized.StartsWith("es-", StringComparison.Ordinal))
        {
            detected = TradeMarkerLanguage.Spanish;
            return true;
        }

        if (normalized.StartsWith("tr-", StringComparison.Ordinal)
            || normalized.StartsWith("tu-", StringComparison.Ordinal))
        {
            detected = TradeMarkerLanguage.Turkish;
            return true;
        }

        if (normalized.StartsWith("ru-", StringComparison.Ordinal))
        {
            detected = TradeMarkerLanguage.Russian;
            return true;
        }

        return false;
    }

    private static string NormalizeLanguageCode(string language)
    {
        return language.Trim()
            .Replace('_', '-')
            .Replace(" ", string.Empty)
            .ToLowerInvariant();
    }
}
