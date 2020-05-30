namespace sjzd.新界面.乡镇精细化.viewModel
{
    class 人员类
    {
        private string name;
        private int _id;
        public 人员类()
        {
        }

        public 人员类(string name, int id)
        {
            Name = name;
            ID = id;
        }
        public 人员类(string name)
        {
            Name = name;
        }

        public string Name
        {
            get
            {
                return this.name;
            }
            set
            {
                this.name = value;
            }
        }

        public int ID
        {
            get
            {
                return this._id;
            }
            set
            {
                this._id = value;
            }
        }


        public override string ToString()
        {
            return this.Name;
        }
    }
}
