import http from 'k6/http';
import { sleep } from 'k6';
import { textSummary } from "https://jslib.k6.io/k6-summary/0.0.1/index.js";

export default function () {
  http.get('https://groffevotacao.azurewebsites.net/Votacao/VotoKubernetes');
  http.get('https://groffevotacao.azurewebsites.net/Votacao/VotoLinux');
  http.get('https://groffevotacao.azurewebsites.net/Votacao/VotoAzure');
  http.get('https://groffevotacao.azurewebsites.net/Votacao/VotoPowerShell');
  sleep(1);
}

export function handleSummary(data) {
    return {
      stdout: textSummary(data, { indent: " ", enableColors: true })
    };
}