using System.Collections.Generic;
using System.Numerics;
using System.Reflection.Emit;

namespace Text_RPG
{
    internal class Program
    {
        static List<Item> ItemList = new List<Item>();
        static Dictionary<int, Item> PlayerInVentory = new Dictionary<int, Item>();
        static Dictionary<int, Item> ItemDic = new Dictionary<int, Item>();
        static int AtNum = -1;
        static string SetStr = "";
        static int NumCheck;

        static void Main(string[] args)
        {
            
            ItemSet();
            Console.WriteLine("닉네임을 설정해주세요");
            string Setname = Console.ReadLine();
            string SetJob = "";
            Player player = new Player();
            int gamestate = 1;
            int inVenItemNum = 0;
            int PlayerAct = 0;
            bool GameOver = true;
            bool bReJob = true;
            Dungeon dungeonLv1 = new Dungeon(20, 10, 1000, 1);
            Dungeon dungeonLv2 = new Dungeon(30, 15, 2000, 3);
            Dungeon dungeonLv3 = new Dungeon(40, 20, 3000, 5);

            Console.WriteLine("직업을 선택해주세요 \n1. 전사, 2. 궁수, 3. 도적");
            
            while (bReJob)
            {
                SetStr = Console.ReadLine();
                AtNum = int.TryParse(SetStr, out NumCheck) ? int.Parse(SetStr) : 0;
                if (AtNum < 1 || AtNum > 3)
                {
                    Console.WriteLine("잘못된 입력입니다.\n");
                }
                else bReJob = false; 
            }
            Console.Clear();
            switch (NumCheck)
            {
                case 1:
                    SetJob = "전사";
                    player.SetPlayer(Setname, SetJob, 10, 10, 100, 15000, ItemDic[0]);
                    break;
                case 2:
                    SetJob = "궁수";
                    player.SetPlayer(Setname, SetJob, 15, 5, 80, 1500, ItemDic[0]);
                    break;
                case 3:
                    SetJob = "도적";
                    player.SetPlayer(Setname, SetJob, 15, 5, 50, 3000, ItemDic[0]);
                    break;
            }

            while (GameOver)
            {
                switch (gamestate)
                {
                    case (int)GameView.MainMenu:
                        Console.WriteLine("스파르타 마을에 오신 여러분 환영합니다.");
                        Console.WriteLine($">>> {player.Name}님 원하시는 행동을 입력해주세요");
                        Console.WriteLine("1. 상태 보기\n2. 인벤토리\n3. 상점\n4. 던전\n5. 휴식하기\n0. 게임종료");
                        Console.WriteLine();
                        PlayerAct = PlayerAction();
                        if (PlayerAct == -1)
                        {
                            Console.WriteLine("잘못된 입력입니다.\n");
                        }
                        else if(PlayerAct == 0) gamestate = PlayerAct;
                        else gamestate = PlayerAct + 1;
                        Console.Clear();
                        break;
                    case (int)GameView.PlayerState:
                        Console.WriteLine("상태 보기\n캐릭터 정보가 표시됩니다.");
                        Console.WriteLine($"Lv. {(player.Level == player.LevelDic.Count ? player.Level + " (만랩)" : player.Level)}");
                        Console.WriteLine($"경험치: {player.Exp} / {player.LevelDic[player.Level]}");
                        Console.WriteLine($"직업: ({player.Job})");
                        Console.WriteLine($"공격력: {player.TotalAk()} ({player.Ak} + {player.itemSlot[1].Power})");
                        Console.WriteLine($"방어력: {player.TotalDef()} ({player.Def} + {player.itemSlot[0].Power})");
                        Console.WriteLine($"체  력: {player.Hp}");
                        Console.WriteLine($"Gold : {player.Gold}");
                        Console.WriteLine($"무기 : {player.itemSlot[1].Name} | + {player.itemSlot[1].Power}");
                        Console.WriteLine($"방어구: : {player.itemSlot[0].Name} | + {player.itemSlot[0].Power}");                        
                        Console.WriteLine("\n0. 나가기\n");
                        PlayerAct = PlayerAction();
                        if (PlayerAct == 0)
                        {
                            gamestate = 1;
                        }
                        else Console.WriteLine("잘못된 입력입니다.\n");
                        break;

                    case (int)GameView.InVentory:
                        Console.WriteLine("인벤토리\n보유 중인 아이템을 관리할 수 있습니다.\n\n");
                        Console.WriteLine($"[착용 아이템]\n무기: {player.itemSlot[1].Name}\n방어구: {player.itemSlot[0].Name}\n\n");
                        Console.WriteLine("[아이템 목록]\n");
                        for (int i = 1; i <= player.InVentoryList.Count; i++)
                        {
                            if (player.InVentoryList[i-1].Type == (int)ItemType.Weapon)
                            {
                                Console.WriteLine($"-{(player.itemSlot[1].Id == player.InVentoryList[i - 1].Id ? "[E]" : "")} {player.InVentoryList[i - 1].Name} | 공격력 +{player.InVentoryList[i - 1].Power} | {player.InVentoryList[i - 1].Text}");                                
                            }
                            else if (player.InVentoryList[i-1].Type == (int)ItemType.Defense)
                            {
                                Console.WriteLine($"-{(player.itemSlot[0].Id == player.InVentoryList[i - 1].Id ? "[E]" : "")} {player.InVentoryList[i - 1].Name} | 방어력 +{player.InVentoryList[i - 1].Power} | {player.InVentoryList[i - 1].Text}");                                
                            }
                        }
                        Console.WriteLine("\n1. 장착 관리\n0. 나가기\n");
                        PlayerAct = PlayerAction();
                        if (PlayerAct == 0)
                        {
                            //메인 메뉴
                            gamestate = 1;
                        }
                        else if (PlayerAct == 1)
                        {
                            Console.WriteLine("인벤토리 - 장착 관리\n보유 중인 아이템을 관리할 수 있습니다.\n\n");
                            Console.WriteLine($"[착용 아이템]\n무기: {player.itemSlot[1].Name}\n방어구: {player.itemSlot[0].Name}\n\n");
                            Console.WriteLine("[아이템 목록]\n");
                            for (int i = 1; i < player.InVentoryList.Count + 1; i++)
                            {
                                if (player.InVentoryList[i-1].Type == (int)ItemType.Weapon)
                                {
                                    Console.WriteLine($"- {i} {(player.itemSlot[1].Id == player.InVentoryList[i - 1].Id ? "[E]" : "")} {player.InVentoryList[i - 1].Name} | 공격력 +{player.InVentoryList[i - 1].Power} | {player.InVentoryList[i - 1].Text}");

                                }
                                else if (player.InVentoryList[i-1].Type == (int)ItemType.Defense)
                                {
                                    Console.WriteLine($"- {i} {(player.itemSlot[0].Id == player.InVentoryList[i - 1].Id ? "[E]" : "")} {player.InVentoryList[i - 1].Name} | 방어력 +{player.InVentoryList[i - 1].Power} | {player.InVentoryList[i - 1].Text}");

                                }
                            }                 
                            
                            inVenItemNum = 0;
                            Console.WriteLine("\n0. 나가기\n");
                            int PlayerEquipAct = PlayerAction();
                            
                            if(PlayerEquipAct == 0)
                            {
                                gamestate = 1;
                                PlayerEquipAct = -1;
                            }
                            else if (player.InVentoryList.Count == 0 || PlayerEquipAct > player.InVentoryList.Count || PlayerEquipAct < 0) Console.WriteLine("잘못된 입력입니다.\n");
                            else
                            {
                                if (player.InVentoryList[PlayerEquipAct - 1].Type == (int)ItemType.Weapon)
                                {
                                    player.itemSlot[1] = player.InVentoryList[PlayerEquipAct - 1];
                                }
                                else if (player.InVentoryList[PlayerEquipAct - 1].Type == (int)(ItemType.Defense))
                                {
                                    player.itemSlot[0] = player.InVentoryList[PlayerEquipAct - 1];
                                }
                            }

                        }
                        break;

                    case (int)GameView.Shop:
                        Console.WriteLine("상점\n필요한 아이템을 얻을 수 있는 상점입니다.\n\n");
                        Console.WriteLine($"[보유 골드]\n{player.Gold} G\n\n");
                        Console.WriteLine("[아이템 목록]\n");
                        foreach (KeyValuePair<int, Item> item in ItemDic)
                        {
                            if(item.Key !=0)
                            {
                                Item shopItem = item.Value;
                                if(shopItem.Type == (int)ItemType.Weapon)
                                    Console.WriteLine($"- {shopItem.Name} | 공격력 +{shopItem.Power} | {shopItem.Text} | {(item.Value.ShopState ? shopItem.Price : "구매완료")} {(item.Value.ShopState ? "G" : "")}");
                                else if(shopItem.Type == (int)ItemType.Defense)
                                {
                                    Console.WriteLine($"- {shopItem.Name} | 방어력 +{shopItem.Power} | {shopItem.Text} | {(item.Value.ShopState ? shopItem.Price : "구매완료")} {(item.Value.ShopState ? "G" : "")}");
                                }
                            }                            
                        }
                        Console.WriteLine("\n1. 아이템 구매\n2. 아이템 판매\n0. 나가기\n");
                        PlayerAct = PlayerAction();
                        if (PlayerAct == 0)
                        {
                            //메인 메뉴
                            gamestate = 1;
                        }
                        else if (PlayerAct == 1)
                        {
                            //아이템 구매
                            Console.WriteLine("상점 - 아이템 구매\n구매할 아이템을 선택해주세요..\n\n");
                            Console.WriteLine($"[보유 골드]\n{player.Gold} G\n\n");
                            Console.WriteLine("[아이템 목록]\n");
                            foreach (KeyValuePair<int, Item> item in ItemDic)
                            {
                                if (item.Key != 0)
                                {
                                    Item shopItem = item.Value;
                                    if (shopItem.Type == (int)ItemType.Weapon)
                                    {
                                        Console.WriteLine($"- {shopItem.Id} {shopItem.Name} | 공격력 +{shopItem.Power} | {shopItem.Text} | {(item.Value.ShopState ? shopItem.Price : "구매완료")} {(item.Value.ShopState ? "G" : "")}");
                                    }
                                    else if (shopItem.Type == (int)ItemType.Defense)
                                    {
                                        Console.WriteLine($"- {shopItem.Id} {shopItem.Name} | 방어력 +{shopItem.Power} | {shopItem.Text} | {(item.Value.ShopState ? shopItem.Price : "구매완료")} {(item.Value.ShopState ? "G" : "")}");
                                    }
                                }
                            }
                            Console.WriteLine("\n0. 나가기\n");
                            int PlayerShopAct = PlayerAction();

                            if (PlayerShopAct == 0)
                            {
                                PlayerAct = 0;
                            }
                            else
                            {
                                if (PlayerShopAct > ItemDic.Count || PlayerShopAct < 0) { Console.WriteLine("잘못된 입력입니다.\n"); }
                                else if (!ItemDic[PlayerShopAct].ShopState)
                                {
                                    Console.WriteLine("이미 구매한 아이템입니다.\n");
                                }
                                else if (player.Gold >= ItemDic[PlayerShopAct].Price)
                                {
                                    player.Gold -= ItemDic[PlayerShopAct].Price;
                                    player.AddInVentory(ItemDic[PlayerShopAct]);
                                    ItemDic[PlayerShopAct].ShopState = false;
                                    Console.WriteLine("구매를 완료했습니다.\n");
                                }
                                else
                                {
                                    Console.WriteLine("골드가 부족합니다.\n");
                                }
                            }
                        }
                        else if (PlayerAct == 2)
                        {
                            //아이템 판매
                            Console.WriteLine("[아이템 목록]\n");
                            Console.WriteLine("상점 - 아이템 판매\n판매할 아이템을 선택해주세요.\n\n");
                            Console.WriteLine($"[보유 골드]\n{player.Gold} G\n\n");
                            Console.WriteLine("[아이템 목록]\n");
                            int num = 1;
                            foreach (Item item in player.InVentoryList)
                            {                                
                                Console.WriteLine($"- {num} {item.Name} | 방어력 +{item.Power} | {item.Text} | {(item.Price)} G ");
                                num++;
                            }
                            Console.WriteLine("\n0. 나가기\n");
                            int PlayerShopAct = PlayerAction();

                            if (PlayerShopAct == 0)
                            {
                                PlayerAct = 0;
                            }
                            else if (player.InVentoryList.Count == 0 || PlayerShopAct > player.InVentoryList.Count || PlayerShopAct < 0) Console.WriteLine("잘못된 입력입니다.\n");
                            else
                            {
                                player.Gold += (int)(player.InVentoryList[PlayerShopAct-1].Price * 0.85f);
                                ItemDic[player.InVentoryList[PlayerShopAct - 1].Id].ShopState = true;
                                if (player.itemSlot[0].Id == player.InVentoryList[PlayerShopAct - 1].Id)
                                {
                                    player.itemSlot[0] = ItemDic[0];
                                }
                                else player.itemSlot[1] = ItemDic[0];
                                player.InVentoryList.Remove(player.InVentoryList[PlayerShopAct - 1]);
                            }
                        }
                        else Console.WriteLine("잘못된 입력입니다.\n");
                        break;

                    case (int)GameView.Dungeon:
                        Console.WriteLine("던전입장\n이곳에서 던전으로 들어가기전 활동을 할 수 있습니다.\n\n");
                        Console.WriteLine($"1. 쉬운 던전 | 방어력 {dungeonLv1.defensivePower} 이상 권장\n");
                        Console.WriteLine($"2. 일반 던전 | 방어력 {dungeonLv2.defensivePower} 이상 권장\n");
                        Console.WriteLine($"3. 어려운 던전 | 방어력 {dungeonLv3.defensivePower} 이상 권장\n");
                        
                        Console.WriteLine("\n0. 나가기\n");
                        int dungeonAct = PlayerAction();
                        
                        if (dungeonAct == 0) gamestate = 1;
                        else if (dungeonAct == 1)
                        {
                            dungeonLv1.Fight(player);
                            dungeonAct = PlayerAction();
                        }
                        else if (dungeonAct == 2)
                        {
                            dungeonLv2.Fight(player);
                            dungeonAct = PlayerAction();
                        }
                        else if (dungeonAct == 3)
                        {
                            dungeonLv3.Fight(player);
                            dungeonAct = PlayerAction();
                        }
                        else Console.WriteLine("잘못된 입력입니다.\n");

                        GameOver = player.DieCheck();
                        break;


                    case (int)GameView.Heals:
                        Console.Write("휴식하기\n500 G 를 내면 최대 체력의 50%를 회복할 수 있습니다. ");
                        Console.WriteLine($"(보유 골드 : {player.Gold} G)\n");
                        Console.WriteLine("\n1. 휴식하기\n0. 나가기\n");                       
                        PlayerAct = PlayerAction();
                        if(PlayerAct == 0) gamestate = 1;
                        else if(PlayerAct == 1)
                        {
                            if (player.Gold >= 500 && player.Hp != player.maxHp)
                            {
                                Console.WriteLine($"체력이 {(player.Hp > player.maxHp / 2 ? player.maxHp - player.Hp : player.maxHp / 2)} 회복되었습니다.\n");
                                player.Hp += player.maxHp / 2;
                                if (player.Hp > player.maxHp) player.Hp = player.maxHp;
                                player.Gold -= 500;
                                gamestate = 1;                                
                                Console.WriteLine("\n0. 나가기\n");
                                PlayerAct = PlayerAction();
                            }
                            else if(player.Hp == player.maxHp)
                            {
                                Console.WriteLine("최대 체력입니다.\n");
                                Console.WriteLine("\n0. 나가기\n");
                                PlayerAct = PlayerAction();
                            }
                            else if(player.Gold < 500)
                            {
                                Console.WriteLine("골드가 부족합니다\n");
                                Console.WriteLine("\n0. 나가기\n");
                                PlayerAct = PlayerAction();
                            }else Console.WriteLine("잘못된 입력입니다.\n");
                        }
                        break;

                    case 0:
                        GameOver = false;
                        break;

                    default:
                        Console.WriteLine("잘못된 입력입니다.\n");
                        gamestate = 1;
                        break;
                }
                player.TotalAk();
                player.TotalDef();
            }            
            
        }
        static void ItemSet()
        {
            ItemList.Add(new Item(0, "없음", "없음", 0, 0, (int)ItemType.Not, true, false));
            ItemList.Add(new Item(1, "수련자 갑옷", "수련에 도움을 주는 갑옷입니다.", 5, 1000, (int)ItemType.Defense, false, true));
            ItemList.Add(new Item(2, "무쇠갑옷", "무쇠로 만들어져 튼튼한 갑옷입니다.", 9, 1800, (int)ItemType.Defense, false, true));
            ItemList.Add(new Item(3, "스파르타의 갑옷", "스파르타의 전사들이 사용했다는 전설의 갑옷입니다.", 15, 3500, (int)ItemType.Defense, false, true));
            ItemList.Add(new Item(4, "낡은 검", "쉽게 볼 수 있는 낡은 검 입니다.", 2, 600, (int)ItemType.Weapon, false, true));
            ItemList.Add(new Item(5, "청동 도끼", "어디선가 사용됐던거 같은 도끼입니다.", 5, 1500, (int)ItemType.Weapon, false, true));
            ItemList.Add(new Item(6, "스파르타의 창", "스파르타의 전사들이 사용했다는 전설의 창입니다.", 7, 2000, (int)ItemType.Weapon, false, true));

            foreach (Item item in ItemList)
            {
                ItemDic.Add(item.Id, item);
            }
        }
        static int PlayerAction()
        {
            Console.Write("원하시는 행동을 입력해주세요.\n>>>");
            SetStr = Console.ReadLine();
            AtNum = int.TryParse(SetStr, out NumCheck) ? int.Parse(SetStr) : -1;
            Console.Clear();
            return AtNum;
        }
    }

    public class Player
    {
        public Dictionary<int,int> LevelDic = new Dictionary<int, int>(); // key = 레벨, Value = 경험치 
        public Item[] itemSlot = new Item[2]; // 0 = 방어구, 1 = 무기
        public List<Item> InVentoryList = new List<Item>(); // 플레이어 인벤토리 리스트
        public int Level {  get; set; } // 플레이어 레벨
        public int Exp {  get; set; } // 플레이어 경험치
        public string Name { get; set; } // 플레이어 이름
        public string Job { get; set; } // 플레이어 직업
        public int Ak { get; set; } // 플레이어 공격력
        public int Def { get; set; } // 플레이어 방어력
        public int Hp { get; set; } // 플레이어 현재 생명력
        public int maxHp { get; set; } // 플레이어 최대 생명력
        public int Gold { get; set; } // 플레이어 골드
        public bool Die { get; set; } // 플레이어 사망여부
        public void SetPlayer(string name, string job, int ak, int def, int hp, int gold, Item SetSlot) // 플레이어 초기화
        {
            Die = false;
            Level = 1;
            Exp = 0;
            for(int i = 1;i <= 10;i++)
            {
                LevelDic.Add(i, i);
            }
            Name = name;
            Job = job;
            Ak = ak;                
            Def = def;                
            Hp = hp;
            maxHp = hp;
            Gold = gold;
            itemSlot[0] = SetSlot;
            itemSlot[1] = SetSlot;
        }
        public void AddInVentory(Item Itemlist) // 상점에서 아이템 구메 시 플레이어 인벤토리에 저장
        {
            InVentoryList.Add(Itemlist);
        }
        public int TotalAk() // 레벨 비례와 아이템 능력치 값을 합한 플레이어 공격력
        {
            int totalAk;
            totalAk = Ak + (int)((Level - 1) * 0.5f) + itemSlot[1].Power;
            return totalAk;
        }
        public int TotalDef() // 레벨 비례와 아이템 능력치 값을 합한 플레이어 방어력
        {
            int totalDefense;
            totalDefense = Def + (int)((Level - 1) * 1) + itemSlot[0].Power;
            return totalDefense;
        }
        public bool DieCheck() // 플레이어 사망 체크
        {
            bool Gameover = true;
            if(Hp <= 0)
            {
                Gameover = false;

                Console.Clear();
                Console.WriteLine("당신은 사망하셨습니다.");
            }
            return Gameover;
        }
        public void AddExp(int DungeonExp) // 던전 클리어 시 플레이어 경험치 획득
        {
            bool Levelup = true;
            Exp += DungeonExp;
            while(Levelup)
            {
                if ((int)LevelDic[Level] <= Exp && Level < (int)LevelDic.Count)
                {                    
                    Exp = (int)LevelDic[Level] % Exp;
                    Level += 1;
                }
                else
                {
                    if (Level == LevelDic.Count) Exp = 0;
                    Levelup = false;
                }
            }            
        }
    }
    public class Item
    {
        public int Id { get; set; } // 아이템 ID 
        public String Name {  get; set; } // 아이템 이름
        public String Text {  get; set; } // 아이템 설명
        public int Power {  get; set; } // 아이템 능력치
        public int Price { get; set; } // 아이템 가격
        public int Type {  get; set; } // 아이템 타입 0 = 방어구, 1 = 무기
        public bool EquipState {  get; set; } // 아이템 장착 여부
        public bool ShopState { get; set; } // 아이템 판매 여부
        public Item(int ItenId, String ItemName, String ItemText, int ItemPower, int ItemPrice, int ItemType, bool ItemEquipStaet, bool ItemShopState)
        {
            Id = ItenId;
            Name = ItemName;
            Text = ItemText;
            Power = ItemPower;
            Price = ItemPrice;
            Type = ItemType;
            EquipState = ItemEquipStaet;
            ShopState = ItemShopState;
        }
    }

    interface IDungeon
    {
        int setDamage { get; set; } // 던전 기본 데미지
        int totalDamage { get; set; } // 최종 데미지
        int defensivePower { get; set; } // 권장 방어력
        int reward { get; set; } // 보상
        int exp { get; set; } // 경험치
        void Fight(Player player); // 
        void Clear(Player player); // 던전 클리어
        void fail(Player player); // 던전 탐험 실패
    }
    public class Dungeon : IDungeon
    {
        public int setDamage { get; set; }
        public int totalDamage { get; set; }
        public int defensivePower { get; set; }
        public int reward { get; set; }
        public int exp { get; set; }

        public Dungeon(int Damage, int DefensivePower, int Rewerd, int Exp) 
        {
            setDamage = Damage;
            defensivePower = DefensivePower;
            reward = Rewerd;
            exp = Exp;
        }
        public void Fight(Player player)
        {
            if (defensivePower > player.Def)
            {
                Random rand = new Random();
                int failnum = rand.Next(1, 10);
                if (failnum <= 4)
                    fail(player);
                else Clear(player);
            }
            else
            {
                Clear(player);
            }

            Console.WriteLine("\n아무 키나 입력하세요.\n");
        }
        public void Clear(Player player)
        {
            Random random = new Random();
            totalDamage = random.Next(setDamage-15,setDamage) - (defensivePower - player.Def);
            Console.WriteLine("던전 클리어\n축하합니다!!\n쉬운 던전을 클리어 하였습니다.");
            Console.WriteLine("[탐험 결과]");
            Console.WriteLine($"받은 데미지: {totalDamage}");
            Console.WriteLine($"체력: {player.Hp} -> {(player.Hp - totalDamage > 0 ? player.Hp - totalDamage : 0)}");
            Console.WriteLine($"Gold: {player.Gold} -> {player.Gold + reward + (int)(player.Ak * 0.2f)}");
            player.Hp -= totalDamage;
            if( player.Hp < 0 ) player.Hp = 0;
            player.Gold += reward + (int)(player.Ak * 0.2f);
            player.AddExp(exp);
        }

        public void fail(Player player)
        {
            Random random = new Random();
            totalDamage = random.Next(setDamage - 15, setDamage) * 2;
            Console.WriteLine("탐험 실패\n클리어 실패\n쉬운 던전 탐험을 실패 하였습니다.");
            Console.WriteLine("[탐험 결과]");
            Console.WriteLine($"체력: {player.Hp} -> {(player.Hp - totalDamage > 0 ? player.Hp - totalDamage : 0)}");
            player.Hp -= totalDamage;
            if (player.Hp < 0) player.Hp = 0;
        }
    }
    public enum GameView
    {
        MainMenu = 1,
        PlayerState,
        InVentory,
        Shop,
        Dungeon,
        Heals
    }

    public enum ItemType
    {
        Not = 0,
        Defense,
        Weapon,
        Sale
    }
}
