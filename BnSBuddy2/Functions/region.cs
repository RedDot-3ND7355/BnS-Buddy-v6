﻿namespace BnSBuddy2.Functions
{
    public class region
    {
        public string name;

        public string value;

        public string appId;

        public region(string n, string v, string a)
        {
            name = n;
            value = v;
            appId = a;
        }

        public override string ToString()
        {
            return name;
        }
    }
}
