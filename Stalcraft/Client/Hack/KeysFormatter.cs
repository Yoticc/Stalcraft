using Interception;

static class KeysFormatter
{
    static KeysFormatter()
    {
        names = new string[100 + 350];
        var values = Enum.GetValues<FormattedKeys>();
        foreach (var value in values)
            names[(short)value + 100] = value.ToString().TrimStart('_').Replace("__", "TEMP").Replace("_", " ").Replace("TEMP", "_");

        SetName(Key.OpenBracket, "[");
        SetName(Key.CloseBracket, "]");
        SetName(Key.Backslash, "\\");
        SetName(Key.Slash, "/");
        SetName(Key.Semicolon, ":");
        SetName(Key.Tilde, "~");
        SetName(Key.Period, ".");
        SetName(Key.Comma, ",");
        SetName(Key.Minus, "-");
        SetName(Key.Plus, "+");

        void SetName(Key key, string name) => names[(short)key + 100] = name;
    }

    static string[] names;

    public static string Formate(Key key) => names[(short)key + 100];

    enum FormattedKeys : short
    {
        mousel = -100,
        mouser = -99,
        mousem = -98,
        btn1 = -97,
        btn2 = -96,

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
        backspc = 14,
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
        lbrack = 26,
        rbrack = 27,
        enter = 28,
        lctrl = 29,
        a = 30,
        s = 31,
        d = 32,
        f = 33,
        g = 34,
        h = 35,
        j = 36,
        k = 37,
        l = 38,
        semicon = 39,
        quote = 40,
        tilde = 41,
        lshift = 42,
        bckslsh = 43,
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
        rshift = 54,
        nummul = 55,
        lalt = 56,
        space = 57,
        capslck = 58,
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
        numlck = 69,
        scrlck = 70,
        num7 = 71,
        num8 = 72,
        num9 = 73,
        nummin = 74,
        num4 = 75,
        num5 = 76,
        num6 = 77,
        numadd = 78,
        num1 = 79,
        num2 = 80,
        num3 = 81,
        num0 = 82,
        numdel = 83,
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
        nocnvrt = 123,
        yen = 125,
        numqqls = 141,
        crcumex = 144,
        at = 145,
        colon = 146,
        undrln = 147,
        kanji = 148,
        stop = 149,
        ax = 150,
        uulbld = 151,
        //NumEnter = 156,
        //RControl = 157,
        section = 167,
        numcm = 179,
        dvne = 181,
        sysrq = 183,
        rmenu = 184,
        func = 196,
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
        lmeta = 219,
        lwwin = 219,
        rmeta = 220,
        rwwin = 220,
        apps = 221,
        power = 222,
        sleep = 223,

        nenter = 284,
        rctrl = 285,

        numdiv = 309,

        ralt = 312,

        arrwup = 328,

        insert = 338,
        delete = 339,

        home = 327,
        pageup = 329,

        larrow = 331,

        rarrow = 333,

        end = 335,
        arwdwn = 336,
        pgdwn = 337,

        lwin = 347,
        rwin = 348,
        cntxt = 349,
    }
}