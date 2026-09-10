using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text.RegularExpressions;
using System.Windows.Forms;

public static class RuSetup
{
    public const string Pack = "SF5_Russian-5.0.8";
    public const string Ver = "5.0.8";
    public static Action<int> Prog;

    [STAThread]
    public static int Main(string[] args)
    {
        try { System.Console.OutputEncoding = System.Text.Encoding.UTF8; }
        catch { }
        string root = GetDataRoot();
        if (args.Length > 0 && Directory.Exists(Path.Combine(args[0], "mods")))
        {
            try
            {
                if (args.Length > 1 && args[1].ToLowerInvariant() == "rollback")
                    Rollback(args[0], Console.WriteLine);
                else
                    Install(GetDataRoot(), args[0], true, Console.WriteLine);
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

    public static string GetDataRoot()
    {
        string exeDir = AppDomain.CurrentDomain.BaseDirectory;
        if (Directory.Exists(Path.Combine(exeDir, "resourcepacks"))) return exeDir;
        string tmp = Path.Combine(Path.GetTempPath(), "SF5RU_" + Ver);
        if (Directory.Exists(tmp)) Directory.Delete(tmp, true);
        Directory.CreateDirectory(tmp);
        var asm = System.Reflection.Assembly.GetExecutingAssembly();
        using (Stream s = asm.GetManifestResourceStream("Payload.Data"))
        using (ZipArchive z = new ZipArchive(s))
            z.ExtractToDirectory(tmp);
        return tmp;
    }

    public static List<CompStatus> CheckAll(string root, string instance)
    {
        List<CompStatus> res = new List<CompStatus>();
        res.Add(CheckGroup("Пак", Path.Combine(root, "resourcepacks", Pack), Path.Combine(instance, "resourcepacks", Pack)));
        res.Add(CheckGroup("Скрипты", Path.Combine(root, "scripts-patch", "scripts"), Path.Combine(instance, "scripts")));
        res.Add(CheckGroup("Задания", Path.Combine(root, "config-patch", "config"), Path.Combine(instance, "config")));
        res.Add(CheckGroup("Датапак", Path.Combine(root, "datapack-patch", "data"), Path.Combine(instance, "global_packs", "required_data", "skyfactory_5", "data")));
        res.Add(CheckGroup("Паки", Path.Combine(root, "datapack-patch", "global"), Path.Combine(instance, "global_packs")));
        return res;
    }

    static CompStatus CheckGroup(string name, string srcBase, string dstBase)
    {
        CompStatus c = new CompStatus();
        c.name = name;
        if (!Directory.Exists(srcBase)) return c;
        foreach (string f in Directory.GetFiles(srcBase, "*", SearchOption.AllDirectories))
        {
            string rel = f.Substring(srcBase.Length).TrimStart(Path.DirectorySeparatorChar);
            string dst = Path.Combine(dstBase, rel);
            c.total++;
            if (!File.Exists(dst)) { c.missing++; continue; }
            if (SameFile(f, dst)) c.ok++; else c.broken++;
        }
        return c;
    }

    static bool SameFile(string a, string b)
    {
        byte[] x = File.ReadAllBytes(a);
        byte[] y = File.ReadAllBytes(b);
        if (x.Length == y.Length)
        {
            bool same = true;
            for (int i = 0; i < x.Length; i++) if (x[i] != y[i]) { same = false; break; }
            if (same) return true;
        }
        if (!a.EndsWith(".json", StringComparison.OrdinalIgnoreCase)) return false;
        try
        {
            var ser = new System.Web.Script.Serialization.JavaScriptSerializer();
            ser.MaxJsonLength = 100 * 1024 * 1024;
            var dx = ser.Deserialize<System.Collections.Generic.Dictionary<string, object>>(NormText(File.ReadAllText(a)));
            var dy = ser.Deserialize<System.Collections.Generic.Dictionary<string, object>>(NormText(File.ReadAllText(b)));
            if (dx.Count != dy.Count) return false;
            foreach (var kv in dx)
            {
                if (!dy.ContainsKey(kv.Key)) return false;
                if (NormText(Str(kv.Value)) != NormText(Str(dy[kv.Key]))) return false;
            }
            return true;
        }
        catch { return false; }
    }

    static string Str(object o)
    {
        if (o == null) return "";
        if (o is string) return (string)o;
        return new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(o);
    }

    static string NormText(string t)
    {
        if (t == null) return "";
        return t.Replace("\r\n", "\n").Replace('\r', '\n').Trim();
    }

    public static string PackState(string instance)
    {
        string opt = Path.Combine(instance, "options.txt");
        if (!File.Exists(opt)) return "пак: включи вручную";
        return File.ReadAllText(opt).Contains(Pack) ? "пак: включён" : "пак: выключен";
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
        int before = BadCount(CheckAll(root, instance));
        if (before == 0) log("Всё уже установлено и цело.");
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
        CopyDir(Path.Combine(root, "datapack-patch", "global"), Path.Combine(instance, "global_packs"), log);
        EnablePack(instance, log);
        int after = BadCount(CheckAll(root, instance));
        log("Проверено после установки, осталось проблем: " + after + ".");
        log("Готово! Перезапусти игру. Язык в игре: Русский.");
    }

    static int BadCount(List<CompStatus> list)
    {
        int n = 0;
        foreach (CompStatus c in list) n += c.missing + c.broken;
        return n;
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
            string t = f.Replace(src, dst);
            if (File.Exists(t) && !File.Exists(t + ".en.bak"))
                File.Copy(t, t + ".en.bak");
            File.Copy(f, t, true);
            n++;
            if (Prog != null) Prog(1);
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
        if (Prog != null) Prog(1);
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
        if (t.Contains(Pack)) log("Пак уже включён.");
        else
        {
            t = Regex.Replace(t, "(resourcePacks:\\[[^\\]]*)(\\])", "$1,\"file/" + Pack + "\"$2");
            log("Пак включён.");
        }
        if (Regex.IsMatch(t, "(?m)^lang:"))
            t = Regex.Replace(t, "(?m)^lang:.*$", "lang:ru_ru");
        else
            t += Environment.NewLine + "lang:ru_ru" + Environment.NewLine;
        File.WriteAllText(opt, t);
        log("Язык игры: русский.");
    }

    public static void Rollback(string instance, Action<string> log)
    {
        int restored = 0;
        int removed = 0;
        foreach (string bak in Directory.GetFiles(instance, "*.en.bak", SearchOption.AllDirectories))
        {
            string orig = bak.Substring(0, bak.Length - 7);
            File.Copy(bak, orig, true);
            File.Delete(bak);
            restored++;
        }
        string root = GetDataRoot();
        foreach (string[] pair in new string[][] {
            new string[] { Path.Combine(root, "scripts-patch", "scripts"), Path.Combine(instance, "scripts") },
            new string[] { Path.Combine(root, "config-patch", "config"), Path.Combine(instance, "config") },
            new string[] { Path.Combine(root, "datapack-patch", "data"), Path.Combine(instance, "global_packs", "required_data", "skyfactory_5", "data") },
            new string[] { Path.Combine(root, "datapack-patch", "global"), Path.Combine(instance, "global_packs") } })
        {
            if (!Directory.Exists(pair[0])) continue;
            foreach (string f in Directory.GetFiles(pair[0], "*", SearchOption.AllDirectories))
            {
                string dst = Path.Combine(pair[1], f.Substring(pair[0].Length).TrimStart(Path.DirectorySeparatorChar));
                if (File.Exists(dst) && !File.Exists(dst + ".en.bak") && FilesEqual(f, dst))
                {
                    File.Delete(dst);
                    removed++;
                }
            }
        }
        string packDir = Path.Combine(instance, "resourcepacks", Pack);
        if (Directory.Exists(packDir)) { Directory.Delete(packDir, true); log("Пак удалён."); }
        string opt = Path.Combine(instance, "options.txt");
        if (File.Exists(opt))
        {
            string t = File.ReadAllText(opt);
            string nt = t.Replace(",\"file/" + Pack + "\"", "").Replace("\"file/" + Pack + "\",", "").Replace("\"file/" + Pack + "\"", "");
            if (nt != t) { File.WriteAllText(opt, nt); log("Пак выключен."); }
        }
        log("Восстановлено: " + restored + ", удалено наших: " + removed + ".");
    }

    static bool FilesEqual(string a, string b)
    {
        byte[] x = File.ReadAllBytes(a);
        byte[] y = File.ReadAllBytes(b);
        if (x.Length != y.Length) return false;
        for (int i = 0; i < x.Length; i++) if (x[i] != y[i]) return false;
        return true;
    }

    public static bool IsSF5(string instance)
    {
        if (Directory.GetFiles(Path.Combine(instance, "mods"), "SkyFactoryTweaks-*.jar").Length > 0) return true;
        if (Directory.Exists(Path.Combine(instance, "global_packs", "required_data", "skyfactory_5"))) return true;
        return false;
    }

    public static bool IsCompatibleVersion(string instance)
    {
        string name = Path.GetFileName(instance.TrimEnd(Path.DirectorySeparatorChar));
        if (name.Contains("5.0.8")) return true;
        try
        {
            string ij = Path.Combine(instance, "instance.json");
            if (File.Exists(ij) && File.ReadAllText(ij).Contains("5.0.8")) return true;
        }
        catch { }
        return false;
    }

    static string LastPathFile()
    {
        return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "SF5Russian", "last.txt");
    }

    public static void SaveLastPath(string p)
    {
        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(LastPathFile()));
            File.WriteAllText(LastPathFile(), p);
        }
        catch { }
    }

    public static string LoadLastPath()
    {
        try
        {
            string f = LastPathFile();
            if (File.Exists(f))
            {
                string p = File.ReadAllText(f).Trim();
                if (Directory.Exists(Path.Combine(p, "mods"))) return p;
            }
        }
        catch { }
        return "";
    }
}

public class CompStatus
{
    public string name;
    public int ok;
    public int missing;
    public int broken;
    public int total;
    public string Text()
    {
        if (total == 0) return name + ": нет данных";
        if (missing == 0 && broken == 0) return name + ": OK (" + total + ")";
        return name + ": чинить " + (missing + broken) + " (нет: " + missing + ", битых: " + broken + ")";
    }
}

public class SetupForm : Form
{
    TextBox pathBox;
    ComboBox combo;
    Label statusLbl;
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
        BackColor = System.Drawing.Color.White;
        try { Icon = System.Drawing.Icon.ExtractAssociatedIcon(Application.ExecutablePath); }
        catch { }

        PictureBox pic = new PictureBox() { Top = 8, Left = 14, Width = 52, Height = 52, SizeMode = PictureBoxSizeMode.Zoom };
        try
        {
            string ip = Path.Combine(root, "resourcepacks", RuSetup.Pack, "pack.png");
            if (File.Exists(ip)) pic.Image = System.Drawing.Image.FromFile(ip);
        }
        catch { }
        Label t = new Label() { Text = "Русификатор SkyFactory 5 (v" + RuSetup.Ver + ")", Top = 10, Left = 74, Width = 460, Font = new System.Drawing.Font("Segoe UI", 13F, System.Drawing.FontStyle.Bold), ForeColor = System.Drawing.Color.FromArgb(30, 60, 120) };
        Label sub = new Label() { Text = "Полный перевод модпака by Qiota", Top = 36, Left = 74, Width = 460, ForeColor = System.Drawing.Color.Gray };
        Label d = new Label() { Text = "Папка instance (где лежат mods, resourcepacks, config):", Top = 52, Left = 14, Width = 520 };
        pathBox = new TextBox() { Top = 74, Left = 14, Width = 360 };
        Button browse = new Button() { Text = "Обзор...", Top = 72, Left = 382, Width = 75 };
        browse.Click += delegate {
            FolderBrowserDialog d2 = new FolderBrowserDialog();
            d2.Description = "Выбери папку instance";
            if (d2.ShowDialog() == DialogResult.OK) { pathBox.Text = d2.SelectedPath; RefreshStatus(); }
        };
        Button find = new Button() { Text = "Найти", Top = 72, Left = 463, Width = 70 };
        find.Click += delegate { RefreshList(); };
        Label cl = new Label() { Text = "Найденные сборки (можно выбрать или указать путь вручную):", Top = 100, Left = 14, Width = 520 };
        combo = new ComboBox() { Top = 120, Left = 14, Width = 519, DropDownStyle = ComboBoxStyle.DropDownList };
        combo.SelectedIndexChanged += delegate {
            if (combo.SelectedItem != null) { pathBox.Text = combo.SelectedItem.ToString(); RefreshStatus(); }
        };
        statusLbl = new Label() { Top = 172, Left = 14, Width = 519, Height = 30, Text = "Статус: выбери папку.", Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold) };
        bakBox = new CheckBox() { Text = "Делать бэкапы оригиналов (.en.bak)", Top = 148, Left = 14, Width = 265, Checked = true };
        goBtn = new Button() { Text = "Установить", Top = 144, Left = 288, Width = 115, Height = 30, Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold) };
        AcceptButton = goBtn;
        goBtn.Click += delegate { Run(); };
        Button rbBtn = new Button() { Text = "Откатить", Top = 144, Left = 409, Width = 124, Height = 30 };
        rbBtn.Click += delegate {
            string inst = pathBox.Text.Trim().Trim('"');
            if (!Directory.Exists(Path.Combine(inst, "mods")))
            {
                MessageBox.Show("В папке нет mods. Проверь путь.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (MessageBox.Show("Вернуть оригинальные файлы и убрать пак?", "Откат", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;
            try
            {
                RuSetup.Rollback(inst, Log);
                RefreshStatus();
                MessageBox.Show("Откат выполнен.", "Готово", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception e)
            {
                Log("ОШИБКА: " + e.Message);
            }
        };
        logBox = new TextBox() { Top = 204, Left = 14, Width = 519, Height = 126, Multiline = true, ReadOnly = true, ScrollBars = ScrollBars.Vertical, Font = new System.Drawing.Font("Consolas", 8.5F), BackColor = System.Drawing.Color.FromArgb(245, 245, 245) };
        bar = new ProgressBar() { Top = 336, Left = 14, Width = 519, Height = 20, Style = ProgressBarStyle.Marquee, Visible = false };

        Controls.Add(pic); Controls.Add(sub); Controls.Add(t); Controls.Add(d); Controls.Add(pathBox);
        Controls.Add(browse); Controls.Add(find); Controls.Add(cl); Controls.Add(combo); Controls.Add(statusLbl); Controls.Add(bakBox);
        Controls.Add(goBtn); Controls.Add(rbBtn); Controls.Add(logBox); Controls.Add(bar);
        RefreshList();
        string last = RuSetup.LoadLastPath();
        if (last != "") { pathBox.Text = last; RefreshStatus(); }
        else RefreshStatus();
    }

    void RefreshStatus()
    {
        string inst = pathBox.Text.Trim().Trim('"');
        if (!Directory.Exists(Path.Combine(inst, "mods")))
        {
            statusLbl.Text = "Статус: нет папки mods.";
            statusLbl.ForeColor = System.Drawing.Color.Gray;
            return;
        }
        List<CompStatus> list = RuSetup.CheckAll(root, inst);
        string s = "";
        int bad = 0;
        foreach (CompStatus c in list)
        {
            s += c.Text() + " | ";
            bad += c.missing + c.broken;
        }
        statusLbl.Text = "Статус: " + s + "\n" + RuSetup.PackState(inst) + ".";
        statusLbl.ForeColor = bad == 0 ? System.Drawing.Color.FromArgb(0, 130, 0) : System.Drawing.Color.FromArgb(180, 60, 0);
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
        if (!RuSetup.IsSF5(inst))
        {
            if (MessageBox.Show("Это не похоже на SkyFactory 5. Продолжить?", "Проверка", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes) return;
        }
        if (!RuSetup.IsCompatibleVersion(inst))
        {
            if (MessageBox.Show("Похоже, версия сборки не 5.0.8 (перевод сделан под неё). Продолжить?", "Версия", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes) return;
        }
        goBtn.Enabled = false;
        bar.Style = ProgressBarStyle.Continuous;
        bar.Maximum = CountFiles();
        bar.Value = 0;
        bar.Visible = true;
        int tick = 0;
        RuSetup.Prog = delegate(int n) {
            bar.Value = System.Math.Min(bar.Maximum, bar.Value + n);
            if (++tick % 25 == 0) Application.DoEvents();
        };
        try
        {
            RuSetup.Install(root, inst, bakBox.Checked, Log);
            RuSetup.SaveLastPath(inst);
            RefreshStatus();
            MessageBox.Show("Готово! Перезапусти игру.\nЯзык в игре: Русский.", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        catch (Exception e)
        {
            Log("ОШИБКА: " + e.Message);
            MessageBox.Show(e.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        RuSetup.Prog = null;
        bar.Visible = false;
        goBtn.Enabled = true;
    }

    int CountFiles()
    {
        int n = 0;
        foreach (string d in new string[] {
            Path.Combine(root, "resourcepacks", RuSetup.Pack),
            Path.Combine(root, "scripts-patch"),
            Path.Combine(root, "config-patch"),
            Path.Combine(root, "datapack-patch") })
        {
            if (Directory.Exists(d)) n += Directory.GetFiles(d, "*", SearchOption.AllDirectories).Length;
        }
        return System.Math.Max(n, 1);
    }
}
