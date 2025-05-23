static class KeysFormatter
{
    static KeysFormatter()
    {
        names = new string[100 + 350];
        var values = Enum.GetValues<FormattedKeys>();
        foreach (var value in values)
            names[(short)value + 100] = value.ToString().TrimStart('_').Replace("__", "TEMP").Replace("_", " ").Replace("TEMP", "_");

        SetName(Keys.OpenBracket, "[");
        SetName(Keys.CloseBracket, "]");
        SetName(Keys.Backslash, "\\");
        SetName(Keys.Slash, "/");
        SetName(Keys.Semicolon, ":");
        SetName(Keys.Tilde, "~");
        SetName(Keys.Period, ".");
        SetName(Keys.Comma, ",");
        SetName(Keys.Minus, "-");
        SetName(Keys.Plus, "+");

        void SetName(Keys key, string name) => names[(short)key + 100] = name;
    }

    static string[] names;

    public static string Formate(Keys key) => names[(short)key + 100];

    enum FormattedKeys : short
    {
        mouse_left = -100,
        mouse_right = -99,
        mouse_middle = -98,
        button_1 = -97,
        button_2 = -96,

        none = 0,
        esc = 1,
        _1 = 2,
        _2 = 3,
        _3 = 4,
        _4 = 5,
        _5 = 6,
        _6 = 7,
        _7 = 8,
        _8 = 9,
        _9 = 10,
        _0 = 11,
        minus = 12,
        plus = 13,
        backspace = 14,
        tab = 15,
        q = 16,
        w = 17,
        e = 18,
        r = 19,
        t = 20,
        y = 21,
        u = 22,
        i = 23,
        o = 24,
        p = 25,
        open_bracket = 26,
        close_bracket = 27,
        enter = 28,
        left_control = 29,
        a = 30,
        s = 31,
        d = 32,
        f = 33,
        g = 34,
        h = 35,
        j = 36,
        k = 37,
        l = 38,
        semicolon = 39,
        quote = 40,
        tilde = 41,
        left_shift = 42,
        backslash = 43,
        z = 44,
        x = 45,
        c = 46,
        v = 47,
        b = 48,
        n = 49,
        m = 50,
        comma = 51,
        period = 52,
        slash = 53,
        right_shift = 54,
        nummul = 55,
        left_alt = 56,
        space = 57,
        caps_lock = 58,
        f1 = 59,
        f2 = 60,
        f3 = 61,
        f4 = 62,
        f5 = 63,
        f6 = 64,
        f7 = 65,
        f8 = 66,
        f9 = 67,
        f10 = 68,
        num_lock = 69,
        scroll_lock = 70,
        num7 = 71,
        num8 = 72,
        num9 = 73,
        num_min = 74,
        num4 = 75,
        num5 = 76,
        num6 = 77,
        num_add = 78,
        num1 = 79,
        num2 = 80,
        num3 = 81,
        num0 = 82,
        num_del = 83,
        f11 = 87,
        f12 = 88,
        f13 = 100,
        f14 = 101,
        f15 = 102,
        f16 = 103,
        f17 = 104,
        f18 = 105,
        kana = 112,
        f19 = 113,
        convert = 121,
        no_convert = 123,
        yen = 125,
        num_qqals = 141,
        circumflex = 144,
        at = 145,
        colon = 146,
        underline = 147,
        kanji = 148,
        stop = 149,
        ax = 150,
        uulabeled = 151,
        //NumEnter = 156,
        //RControl = 157,
        section = 167,
        numcomma = 179,
        divine = 181,
        sysrq = 183,
        right_menu = 184,
        function = 196,
        pause = 197,
        //Home = 199,
        up = 200,
        prior = 201,
        left = 203,
        right = 205,
        //End = 207,
        down = 208,
        next = 209,
        //Insert = 210,
        //Delete = 211,
        clear = 218,
        left_meta = 219,
        left_wwin = 219,
        right_meta = 220,
        right_wwin = 220,
        apps = 221,
        power = 222,
        sleep = 223,

        num_enter = 284,
        right_control = 285,

        num_div = 309,

        right_alt = 312,

        arrow_up = 328,

        insert = 338,
        delete = 339,

        home = 327,
        page_up = 329,

        arrow_left = 331,

        arrow_right = 333,

        end = 335,
        arrow_down = 336,
        page_down = 337,

        left_win = 347,
        right_win = 348,
        context_menu = 349,
    }
}