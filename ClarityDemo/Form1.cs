using Clarity;

namespace ClarityDemo
{
    public partial class Form1 : Form, IClarityProvider
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            ClarityManager.Show(0, "test", "description test goes here", this);
        }

        public List<IClarityProvider> GetChildClarityProviders(int state)
        {
            return new();
        }

        public List<ClarityItem> GetClarityItems(Int32 state)
        {
            List<ClarityItem> response = new List<ClarityItem>();

            response.Add(new ClarityItem("Quick Launch", "Use this to quickly find commands", null, button1));
            response.Add(new ClarityItem("My Profile", "View you profile information, including settings.", null, button2));

            //foreach (ManagerBase manager in _managers)
            //    response.AddRange(manager.GetClarityItems(state));

            //switch ((ClarityState)state)
            //{
            //    case ClarityState.FirstTimeRunning:
            //        //response.Add(new ClarityItem("File Menu", "Contains file commands", new Uri("http://www.github.com"), fileToolStripMenuItem));
            //        break;
            //    case ClarityState.IntroducingMotionEnvironment:

            //        break;
            //    case ClarityState.Simple_Overview:

            //        break;
            //    case ClarityState.Simple_Harwdare:
            //        break;
            //}

            return response;
        }
    }
}
