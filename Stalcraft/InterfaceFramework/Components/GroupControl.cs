using System.Drawing;

class GroupControl : Control
{
    public GroupControl(Point? location = null) : this(location, new Size(64, 64)) { }

    public GroupControl(Point? location, Size? size) : base(location, size) { }
}