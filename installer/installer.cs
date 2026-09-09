using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;

public static class RuSetup
{
    public const string Pack = "SF5_Russian-5.0.8";
    public const string Ver = "5.0.8";

    [STAThread]
    public static int Main(string[] args)
    {
        string root = AppDomain.CurrentDomain.BaseDirectory;
        if (args.Length > 0 && Directory.Exists(Path.Combine(args[0], "mods")))
        {
            try
            {
                Install(root, args[0], true, Console.WriteLine);
                return 0;
            }
            catch (Exception e)
            {
                Console.WriteLine("ERROR: " + e.Message);
                return 1;
            }
        }
        Application.EnableVisualStyles();
        Application.Run(new SetupForm(root));
        return 0;
    }

    public static string DetectInstance()
    {
        List<string> all = AllInstances();
        return all.Count > 0 ? all[0] : "";
    }

    public static List<string> AllInstances()
    {
        List<string> found = new List<string>();
        string home = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        string appdata = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        string[] fixed_ = new string[] {
            Path.Combine(home, ".minecraftx", "instances", "SkyFactory 5-5.0.8"),
            Path.Combine(appdata, ".minecraft")
        };
        foreach (string p in fixed_)
            if (Directory.Exists(Path.Combine(p, "mods")) && !found.Contains(p)) found.Add(p);
        foreach (string base_ in new string[] {
            Path.Combine(home, "curseforge", "minecraft", "Instances"),
            Path.Combine(appdata, "PrismLauncher", "instances"),
            Path.Combine(appdata, "ATLauncher", "Instances") })
        {
            if (!Directory.Exists(base_)) continue;
            foreach (string d in Directory.GetDirectories(base_))
                if (d.IndexOf("SkyFactory", StringComparison.OrdinalIgnoreCase) >= 0
                    && Directory.Exists(Path.Combine(d, "mods")) && !found.Contains(d)) found.Add(d);
        }
        return found;
    }

    public static void Install(string root, string instance, bool backups, Action<string> log)
    {
        if (!Directory.Exists(Path.Combine(instance, "mods")))
            throw new Exception("В папке нет mods: " + instance);
        log("Папка: " + instance);
        CopyDir(Path.Combine(root, "resourcepacks", Pack), Path.Combine(instance, "resourcepacks", Pack), log);
        CopyFile(root, instance, Path.Combine("scripts-patch", "scripts", "tooltips.zs"), Path.Combine("scripts", "tooltips.zs"), backups, log);
        CopyFile(root, instance, Path.Combine("scripts-patch", "scripts", "globals.zs"), Path.Combine("scripts", "globals.zs"), backups, log);
        CopyFile(root, instance, Path.Combine("scripts-patch", "scripts", "colors", "content", "registry", "item_registry.zs"), Path.Combine("scripts", "colors", "content", "registry", "item_registry.zs"), backups, log);
        string itemsDir = Path.Combine(root, "scripts-patch", "scripts", "colors", "items");
        string[] itemFiles = Directory.Exists(itemsDir) ? Directory.GetFiles(itemsDir, "*.zs") : new string[0];
        log("items-файлов найдено: " + itemFiles.Length + " в " + itemsDir);
        foreach (string f in itemFiles)
            CopyFile(root, instance, f.Substring(root.Length).TrimStart(Path.DirectorySeparatorChar), Path.Combine("scripts", "colors", "items", Path.GetFileName(f)), backups, log);
        CopyFile(root, instance, Path.Combine("config-patch", "config", "checklist", "tasks.txt"), Path.Combine("config", "checklist", "tasks.txt"), backups, log);
        CopyDir(Path.Combine(root, "datapack-patch", "data"), Path.Combine(instance, "global_packs", "required_data", "skyfactory_5", "data"), log);
        EnablePack(instance, log);
        log("Готово! Перезапусти игру. Язык в игре: Русский.");
    }

    static void CopyDir(string src, string dst, Action<string> log)
    {
        if (!Directory.Exists(src)) throw new Exception("Нет исходников: " + src);
        Directory.CreateDirectory(dst);
        foreach (string d in Directory.GetDirectories(src, "*", SearchOption.AllDirectories))
            Directory.CreateDirectory(d.Replace(src, dst));
        int n = 0;
        foreach (string f in Directory.GetFiles(src, "*", SearchOption.AllDirectories))
        {
            File.Copy(f, f.Replace(src, dst), true);
            n++;
        }
        log("Скопировано файлов: " + n);
    }

    static void CopyFile(string root, string instance, string relSrc, string relDst, bool backups, Action<string> log)
    {
        string src = Path.Combine(root, relSrc);
        string dst = Path.Combine(instance, relDst);
        if (!File.Exists(src)) throw new Exception("Нет файла: " + src);
        Directory.CreateDirectory(Path.GetDirectoryName(dst));
        if (backups && File.Exists(dst) && !File.Exists(dst + ".en.bak"))
            File.Copy(dst, dst + ".en.bak");
        File.Copy(src, dst, true);
    }

    static void EnablePack(string instance, Action<string> log)
    {
        string opt = Path.Combine(instance, "options.txt");
        if (!File.Exists(opt))
        {
            log("options.txt нет — включи пак вручную в меню.");
            return;
        }
        string t = File.ReadAllText(opt);
        log("options имеет resourcePacks: " + t.Contains("resourcePacks:").ToString());
        if (t.Contains(Pack)) { log("Пак уже включён."); return; }
        t = Regex.Replace(t, "(resourcePacks:\\[[^\\]]*)(\\])", "$1,\"file/" + Pack + "\"$2");
        File.WriteAllText(opt, t);
        log("Пак включён.");
    }
}

public class SetupForm : Form
{
    TextBox pathBox;
    ComboBox combo;
    TextBox logBox;
    ProgressBar bar;
    CheckBox bakBox;
    Button goBtn;
    string root;

    public SetupForm(string root_)
    {
        root = root_;
        Text = "Русификатор SkyFactory 5 v" + RuSetup.Ver + " by Qiota";
        Width = 560;
        Height = 420;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        StartPosition = FormStartPosition.CenterScreen;

        Label t = new Label() { Text = "Русификатор SkyFactory 5 (v" + RuSetup.Ver + ")", Top = 12, Left = 14, Width = 520, Font = new System.Drawing.Font("Segoe UI", 12F) };
        Label d = new Label() { Text = "Папка instance (где лежат mods, resourcepacks, config):", Top = 52, Left = 14, Width = 520 };
        pathBox = new TextBox() { Top = 74, Left = 14, Width = 360 };
        Button browse = new Button() { Text = "Обзор...", Top = 72, Left = 382, Width = 75 };
        browse.Click += delegate {
            FolderBrowserDialog d2 = new FolderBrowserDialog();
            d2.Description = "Выбери папку instance";
            if (d2.ShowDialog() == DialogResult.OK) pathBox.Text = d2.SelectedPath;
        };
        Button find = new Button() { Text = "Найти", Top = 72, Left = 463, Width = 70 };
        find.Click += delegate { RefreshList(); };
        Label cl = new Label() { Text = "Найденные сборки (можно выбрать или указать путь вручную):", Top = 100, Left = 14, Width = 520 };
        combo = new ComboBox() { Top = 120, Left = 14, Width = 519, DropDownStyle = ComboBoxStyle.DropDownList };
        combo.SelectedIndexChanged += delegate {
            if (combo.SelectedItem != null) pathBox.Text = combo.SelectedItem.ToString();
        };
        bakBox = new CheckBox() { Text = "Делать бэкапы оригиналов (.en.bak)", Top = 148, Left = 14, Width = 320, Checked = true };
        goBtn = new Button() { Text = "Установить", Top = 144, Left = 382, Width = 151, Height = 30 };
        goBtn.Click += delegate { Run(); };
        logBox = new TextBox() { Top = 180, Left = 14, Width = 519, Height = 150, Multiline = true, ReadOnly = true, ScrollBars = ScrollBars.Vertical };
        bar = new ProgressBar() { Top = 336, Left = 14, Width = 519, Height = 20, Style = ProgressBarStyle.Marquee, Visible = false };

        Controls.Add(t); Controls.Add(d); Controls.Add(pathBox);
        Controls.Add(browse); Controls.Add(find); Controls.Add(cl); Controls.Add(combo); Controls.Add(bakBox);
        Controls.Add(goBtn); Controls.Add(logBox); Controls.Add(bar);
        RefreshList();
    }

    void RefreshList()
    {
        List<string> all = RuSetup.AllInstances();
        combo.Items.Clear();
        foreach (string p in all) combo.Items.Add(p);
        if (all.Count > 0) combo.SelectedIndex = 0;
        else Log("Сборки не найдены автоматически — укажи путь вручную.");
    }

    void Log(string s) { logBox.AppendText(s + Environment.NewLine); }

    void Run()
    {
        string inst = pathBox.Text.Trim().Trim('"');
        if (!Directory.Exists(Path.Combine(inst, "mods")))
        {
            MessageBox.Show("В папке нет mods. Проверь путь.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }
        goBtn.Enabled = false;
        bar.Visible = true;
        try
        {
            RuSetup.Install(root, inst, bakBox.Checked, Log);
            MessageBox.Show("Готово! Перезапусти игру.\nЯзык в игре: Русский.", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        catch (Exception e)
        {
            Log("ОШИБКА: " + e.Message);
            MessageBox.Show(e.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        bar.Visible = false;
        goBtn.Enabled = true;
    }
}
