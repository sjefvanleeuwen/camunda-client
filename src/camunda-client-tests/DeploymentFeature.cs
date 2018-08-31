using System;
using Xbehave;
using Xunit;
using CamundaClient.Service;

namespace camunda_client_tests
{
    public class DeploymentFeature
    {
        [Scenario]
        public void NamedResource(string resourceName, string deployedResourceName)
        {
            "Given the resource unittestfile.bpmn"
                .x(()=> resourceName = "unittestfile.bpmn");

            "When I Deploy the process"
                .x(()=> deployedResourceName = RepositoryService.GetResourceFiles()[0].FileName);

            "Then the deployed filename is unittestfile.bpmn"
                .x(()=> Xunit.Assert.Equal(resourceName,deployedResourceName));
        }
    }
}
