using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using OrchardCore.Deployment;
using OrchardCore.Lucene.Model;
using OrchardCore.Settings;

namespace OrchardCore.Lucene.Deployment
{
    public class LuceneIndexDeploymentSource : IDeploymentSource
    {
        private readonly LuceneIndexSettingsService _luceneIndexSettingsService;

        public LuceneIndexDeploymentSource(LuceneIndexSettingsService luceneIndexSettingsService, ISiteService siteService)
        {
            _luceneIndexSettingsService = luceneIndexSettingsService;
        }

        public async Task ProcessDeploymentStepAsync(DeploymentStep step, DeploymentPlanResult result)
        {
            var luceneIndexStep = step as LuceneIndexDeploymentStep;

            if (luceneIndexStep == null)
            {
                return;
            }

            var indexSettings = await _luceneIndexSettingsService.GetSettingsAsync();

            var data = new JArray();
            
            foreach (var index in indexSettings) {
                var indexSettingsDict = new Dictionary<string, LuceneIndexSettings>();
                indexSettingsDict.Add(index.IndexName, index);
                data.Add(JObject.FromObject(indexSettingsDict));
            }

            // Adding Lucene settings
            result.Steps.Add(new JObject(
                new JProperty("name", "lucene-index"),
                new JProperty("Indices", data)
            ));
        }
    }
}
