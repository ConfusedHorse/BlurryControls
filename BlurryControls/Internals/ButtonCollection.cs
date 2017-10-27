using System.Windows.Controls;

namespace BlurryControls.Internals
{
    public class ButtonCollection : System.Collections.CollectionBase
    {
        public void Add(Button button)
        {
            List.Add(button);
        }

        public void Remove(int index)
        {
            if (index <= Count - 1 && index >= 0)
            {
                List.RemoveAt(index);
            }
        }

        public Button Item(int index)
        {
            return (Button)List[index];
        }
    }
}