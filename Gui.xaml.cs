using DangerDodger.Classes;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;

namespace DangerDodger
{
    /// <summary>
    /// Interaction logic for Gui.xaml
    /// </summary>
    public partial class Gui : UserControl
    {
        public Gui()
        {
            InitializeComponent();
            List<CheckBoxListItem> defaultBossesValues = DangerDodger.bosses.Select(b => new CheckBoxListItem() { Text = b, IsChecked = true }).ToList();
            if (DangerDodgerSettings.Instance.Bosses == null)
            {
                DangerDodgerSettings.Instance.Bosses = new List<CheckBoxListItem>();
            }
            foreach (CheckBoxListItem item in defaultBossesValues)
            {
                if (!DangerDodgerSettings.Instance.Bosses.Select(i => i.Text).Contains(item.Text))
                    DangerDodgerSettings.Instance.Bosses.Add(item);
            }
        }
    }
}
