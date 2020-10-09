using System;

namespace TwitterLib
{
    

    public interface ITrackerManager
    {
        void OnNewStreamMessage(object sender, EventArgs e);
        object GetTrackerResults();
        bool Initialize();

    }
}
