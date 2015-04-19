using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Color = System.Drawing.Color;
using LeagueSharp;
using LeagueSharp.Common;

namespace Teemo
{
    internal class Program
    {
        public static Menu Menu { get; set; }

        private static Obj_AI_Hero Player
        {
            get { return ObjectManager.Player; }
        }

        private static Orbwalking.Orbwalker Orbwalker;

        private static Spell Q, W, R;

        private static int Combo;

        private static int Mixed;

        private static void Main(string[] args)
        {
            CustomEvents.Game.OnGameLoad += Game_OnGameLoad;
        }

        private static void Game_OnGameLoad(EventArgs args)
        {
            if (Player.ChampionName != "Teemo")
                return;

            Q = new Spell(SpellSlot.Q, 680);
            W = new Spell(SpellSlot.W);
            R = new Spell(SpellSlot.R, 230);


            Menu = new Menu(Player.ChampionName, Player.ChampionName, true);

            Menu orbwalkerMenu = Menu.AddSubMenu(new Menu("Orbwalker", "Orbwalker"));
            Orbwalker = new Orbwalking.Orbwalker(orbwalkerMenu);

            Menu ts = Menu.AddSubMenu(new Menu("Target Selector", "Target Selector"));
            TargetSelector.AddToMenu(ts);

            Menu comboMenu = Menu.AddSubMenu(new Menu("Combo", "Combo"));
            comboMenu.AddItem(new MenuItem("useQCombo", "Use Q").SetValue(true));
            comboMenu.AddItem(new MenuItem("useW", "Use W").SetValue(true));
            comboMenu.AddItem(new MenuItem("Combo", "Combo").SetValue(new KeyBind(32, KeyBindType.Press)));

            Menu mixedMenu = Menu.AddSubMenu(new Menu("Harass", "Harass"));
            mixedMenu.AddItem(new MenuItem("useQHarass", "Use Q").SetValue(true));
            mixedMenu.AddItem(new MenuItem("Harass", "Harass").SetValue(new KeyBind("C".ToCharArray()[0], KeyBindType.Press)));

            Menu fleeMenu = Menu.AddSubMenu(new Menu("Flee", "Flee"));
            fleeMenu.AddItem(new MenuItem("useW", "Use W").SetValue(true));
            fleeMenu.AddItem(new MenuItem("useR", "Use R").SetValue(true));
            fleeMenu.AddItem(new MenuItem("Flee", "Flee").SetValue(new KeyBind("T".ToCharArray()[0], KeyBindType.Press)));

            Menu.AddToMainMenu();

            Drawing.OnDraw += Drawing_OnDraw;

            Game.OnUpdate += Game_OnGameUpdate;

            Game.PrintChat("<font color=\"#66FF33\"><b>Simple Teemo LOADED!</b></font>");

        }

        private static void Flee()
        {
            if (!Menu.Item("useW").GetValue<bool>())
                return;

            ObjectManager.Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);

            if (W.IsReady())
            {
                W.Cast(Player);
            }

            if (!Menu.Item("useR").GetValue<bool>())
                return;
            if (R.IsReady())
            {
                R.Cast(Player.Position);
            }
        }

        private static void Game_OnGameUpdate(EventArgs args)
        {
            if (Player.IsDead)
                return;

            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo)
            {
                BlindingDart();
                ScoutRun();
            }

            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Mixed)
            {
                BlindingDart();
            }

            if (Menu.Item("Flee").GetValue<KeyBind>().Active)
            {
                Flee();
            }
        }


        private static void Drawing_OnDraw(EventArgs args)
        {
            if (Player.IsDead)
                return;

            if (Q.IsReady())
            {
                Utility.DrawCircle(Player.Position, 680, Color.LawnGreen);
            }
            else
            {
                Utility.DrawCircle(Player.Position, 680, Color.Red);
            }
        }

        private static void BlindingDart()
        {
            if (!Menu.Item("useQCombo").GetValue<bool>())
                return;

            Obj_AI_Hero target = TargetSelector.GetTarget(680, TargetSelector.DamageType.Magical);

            if (Q.IsReady())
            {
                if (target.IsValidTarget(Q.Range))
                {
                    Q.CastOnUnit(target);
                }
            }
        }

        private static void ScoutRun()
        {
            if (!Menu.Item("useW").GetValue<bool>())
                return;

            ObjectManager.Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);

            if (W.IsReady())
            {
                W.Cast(Player);
            }
        }
    }

}