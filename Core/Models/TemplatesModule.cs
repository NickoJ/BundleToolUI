using System;
using System.IO;
using Newtonsoft.Json;

namespace BundleToolUI.Models
{
    public sealed class TemplatesModule
    {

        public string CurrentTemplatePath { get; private set; }

        public bool TryLoadTemplate(in string path, out Template template)
        {
            try
            {
                var text = File.ReadAllText(path);
                template = JsonConvert.DeserializeObject<Template>(text);
                CurrentTemplatePath = path;
                return true;
            }
            catch
            {
                template = default;
                return false;
            }
        }
        
        public void SaveTemplate(in string path, in Template template)
        {
            if (template is null) throw new ArgumentNullException(nameof(template));
            
            var text = JsonConvert.SerializeObject(template);
            File.WriteAllText(path, text);
            CurrentTemplatePath = path;
        }

    }
}