using Newtonsoft.Json;
using System;
using System.Collections.Generic;


namespace Ght84.AppsLauncherLibrary.Helpers
{
    public class ContextArgumentHelper
    {
        private ContextArguments _contextArguments; 

        public ContextArgumentHelper()
        {
            _contextArguments = new ContextArguments();
        }

        public ContextArguments ContextArguments
        {
            get { return _contextArguments; }
        }


        public string ConvertToJsonMessage(bool encodeBase64AllValues = true)
        {
            Dictionary<string,string> dict = new Dictionary<string,string>();   
            foreach(var item in _contextArguments)
            {
                if (encodeBase64AllValues)
                    dict.Add(item.Key, Base64Helper.Base64Encode(item.Value.Value));
                else
                    dict.Add(item.Key, item.Value.Value);
            }

            return JsonConvert.SerializeObject(dict);
        }


        public void SetFromJsonMessage(string jsonMessage, bool decodeBase64AllValues = true)
        {

            if (string.IsNullOrEmpty(jsonMessage)) return;


            Dictionary<string, string> kpvList = JsonConvert.DeserializeObject<Dictionary<string,string>>(jsonMessage);

            _contextArguments.Clear();

            foreach (var kpv in kpvList)
            {
                if (decodeBase64AllValues)
                    _contextArguments.Add(kpv.Key.ToUpper(), Base64Helper.Base64Decode(kpv.Value));
                else
                    _contextArguments.Add(kpv.Key.ToUpper(), kpv.Value);
            }
        }


        public void SetFromStringOfArgs(string stringOfArgs, char argSeparator = '|', string codeValueSeparator = ":=")
        {
            if (string.IsNullOrEmpty(stringOfArgs)) return;

            _contextArguments.Clear();

            string[] args = stringOfArgs.Split(argSeparator);
            string[] kvp = null;
            foreach (string entry in args)
            {
                kvp = entry.Split(new string[] { codeValueSeparator }, StringSplitOptions.None);
                if (kvp.Length > 1)
                {           
                        _contextArguments.Add(kvp[0].ToUpper(), kvp[1]);
                }
                else if (kvp.Length == 1)
                {              
                        _contextArguments.Add(kvp[0].ToUpper(), "");
                }
            }

        }


    }


    public class ContextArgument
    {
        public string Code { get; set; }
        public string Value { get; set; }

        public ContextArgument(string code, string value)
        {
            Code = code;
            Value = value;
        }       
    }

    public class ContextArguments : Dictionary<string, ContextArgument>
    {
        //private Dictionary<string, ContextArgument> _contextArguments = new Dictionary<string, ContextArgument>();

        public Dictionary<string, ContextArgument> AllContextArguments
        { 
            get { return this; }
        }

        
        public bool HasCode(string code)
        {
            code = code.ToUpper();
            if (base.ContainsKey(code))
                return true;
            else
                return false;
        }

        public string GetValue(string code)
        {

            code = code.ToUpper();
            if (base.ContainsKey(code))
                return base[code].Value;
            else
                return null;
        }


        public void Add(string code, string value)
        {
            base.Add(code, new ContextArgument(code, value));  
        }

        public void Add(ContextArgument contextArgument)
        {
            base.Add(contextArgument.Code, contextArgument);

            //_contextArguments.Add(contextArgument.Code, contextArgument);
        }

        //public  void Clear()
        //{
        //    base.Clear();

        //    //_contextArguments = new Dictionary<string, ContextArgument>();
        //}

       

    }




    //public class ArgumentProviderHelper
    //{
    //    private readonly Dictionary<string, Argument> arguments;

    //    public ArgumentProviderHelper()
    //    {
    //         arguments = new Dictionary<string, Argument>();
    //    }

    //    public string Get(string key)
    //    {

    //        key = key.ToUpper();
    //        if (arguments.ContainsKey(key))
    //            return this.arguments[key].Value;
    //        else
    //            return null;
    //    }

    //    public bool HasKey(string key)
    //    {

    //        key = key.ToUpper();
    //        if (arguments.ContainsKey(key))
    //            return true;
    //        else
    //            return false;
    //    }


    //    public Dictionary<string, Argument> GetDictionaryWithEncodedBase64Values()
    //    {
    //        Dictionary<string, Argument> newArgs = new Dictionary<string, Argument>();

    //        foreach(Argument arg in arguments.Values)
    //        {
    //            newArgs.Add(arg.Key, new Argument(arg.Key, Base64Helper.Base64Encode(arg.Value)) );
    //        }

    //        return newArgs;
    //    }

    //    public Dictionary<string, Argument> GetDictionaryWithDecodedBase64Values()
    //    {
    //        Dictionary<string, Argument> newArgs = new Dictionary<string, Argument>();

    //        foreach (Argument arg in arguments.Values)
    //        {
    //            newArgs.Add(arg.Key, new Argument(arg.Key, Base64Helper.Base64Decode(arg.Value)));
    //        }

    //        return newArgs;
    //    }



    //    public string[] All(string separator = "=")
    //    {
    //        return this.arguments.Select(c => $"{c.Value.Key}{separator}{c.Value.Value}").ToArray();
    //    }

    //    //public string[] AllWithEncodedBase64Values(string separator = "=")
    //    //{
    //    //    return this.arguments.Select(c => $"{c.Key}{separator}{Base64Helper.Base64Encode(c.Value)}").ToArray();
    //    //}

    //    //public string[] AllWithDecodedBase64Values(string separator = "=")
    //    //{
    //    //    return this.arguments.Select(c => $"{c.Key}{separator}{Base64Helper.Base64Decode(c.Value)}").ToArray();
    //    //}

    //    public void SetArgs(string e, char separator = '|')
    //    {
    //        arguments.Clear();
    //        string[] args = e.Split(separator);
    //        string[] kvp = null;
    //        foreach (string entry in args)
    //        {
    //            kvp = entry.Split('=');
    //            if (kvp.Length > 1)
    //            {
    //                arguments.Add(kvp[0].ToUpper(),  new Argument( kvp[0].ToUpper(), kvp[1]));
    //            }
    //            else if (kvp.Length == 1)
    //            {
    //                arguments.Add(kvp[0].ToUpper(), new Argument(kvp[0].ToUpper(), "") );
    //            }

    //        }
    //    }

    //    public void SetArgs(string[] args)
    //    {
    //        arguments.Clear();
    //        string[] kvp = null;
    //        foreach (string entry in args)
    //        {
    //            kvp = entry.Split('=');
    //            if (kvp.Length > 1)
    //            {
    //                arguments.Add(kvp[0].ToUpper(), new Argument(kvp[0].ToUpper(), kvp[1]));
    //            }
    //            else if (kvp.Length == 1)
    //            {
    //                arguments.Add(kvp[0].ToUpper(), new Argument(kvp[0].ToUpper(), ""));
    //            }
    //        }

    //    }

    //}
}
