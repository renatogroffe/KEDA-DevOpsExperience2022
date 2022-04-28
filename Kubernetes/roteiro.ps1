kubectl create namespace keda

helm repo add kedacore https://kedacore.github.io/charts

helm install keda kedacore/keda --namespace keda

kubectl create namespace contagem

kubectl apply -f .\workercontagem-secret.yml -n contagem

kubectl apply -f .\workercontagem-deployment.yml -n contagem

kubectl apply -f .\workercontagem-scaledobject.yml -n contagem