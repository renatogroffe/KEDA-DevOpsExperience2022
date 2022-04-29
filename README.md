# KEDA-DevOpsExperience2022
Exemplo de utilização de **KEDA (Kubernetes Event-driven Autoscaling)** apresentado durante a **edição de Abril-2022** do **DevOps Experience**.

Worker Service para consumo de **eventos do Azure Event Hubs** (imagem **renatogroffe/workerquestaoeventhub-dotnet6**) - é justamente esta aplicação que será escalada via **KEDA**:

**https://github.com/renatogroffe/DotNet6-Worker-AzureEventHubs-SqlServer-Dapper-Processor_Questoes**

Projeto que serviu de base para o **envio de eventos ao Azure Event Hubs** (imagem **renatogroffe/sitequestaoeventhub-dotnet6**):

**https://github.com/renatogroffe/ASPNETCore6-MVC-AzureEventHubs_SiteQuestao**
