# TaskForge

![](foder/assets.png)

## Instruções para execução

Instalações necessárias:

- sdk do .net 8
- docker

**Rodando a aplicação:**

Acessando a pasta Src do projeto rode:

`docker-compose up`  
            isso vai criar os containers da aplikcação e do postgress

`dotnet ef database update --startup-project Api --project Infrastructure`  
            criação da base, tabelas e seed de dados na tabela Users (exemplos para os testes aqui)

`dot net run` (http://localhost:2606) ou d`ocker-compose up --build` (host: localhost:8080)

[aqui, collection com todos os endpoints disponíveis](assets/TaskForge.postman_collection.json)

[aqui, lista de todos os ususário que são carregados via seed, para user nos testes](assets/Users.png)

**Testando a aplicação:**

Acessar a pasta Tests e rodar  
        `dotnet test`

---

## Fase 2 - sugestões ao _PO_ visando o refinamento para futuras implementações ou melhorias

Durante a fase de refinamento da API, identifiquei diversas melhorias e funcionalidades adicionais que podem aprimorar a experiência do usuário e a robustez do sistema. Abaixo estão os pontos a serem considerados:

**1\. Restrição de remoção de projetos**  
    Além de impedir a remoção de projetos com tarefas pendentes, devemos restringir essa ação apenas ao autor do projeto ou a usuários com a função de gerente.

**2\. Observações das tarefas**  
    Acredito que manter informações de observações das tarefas na tabela de histórico de alterações pode gerar confusão. Sugiro separar as observações em uma estrutura de banco de dados própria, tornando a gestão desses dados mais clara e organizada.

**3\. Remoção de Tarefas**  
    Devemos avaliar se permitiremos a remoção de tarefas mesmo que já tenham sido concluídas, tenham alta prioridade ou não tenham sido modificadas há algum tempo. Essa flexibilidade pode ser vantajosa em alguns cenários, mas precisamos considerar os impactos e possíveis cenários de uso.

**4\. Edição de Projeto**  
    Adicionar um endpoint para edição de projetos permitirá aos usuários atualizarem informações relevantes dos projetos, como título, descrição e prazos.

**5\. Status dos projetos**  
    Implementar status para os projetos, como "aguardando refinamento", "em execução", "pendente", "concluído" e "removido", fornecerá uma visão mais clara do progresso e do estado atual de cada projeto.

**6\. Histórico de atualizações geral**  
    Criar um histórico de atualizações geral que abranja todas as entidades da aplicação nos permitirá rastrear e auditar todas as mudanças realizadas no sistema de forma mais abrangente.

**7\. Gestão de Comentários**  
    Separar as observações das tarefas em uma estrutura própria nos possibilitará gerenciar os comentários de forma mais eficiente e distinguir claramente entre as alterações feitas na tarefa e os comentários adicionados pelos usuários.

**8\. Endpoints para gestão de usuários**  
    Adicionar endpoints para a gestão de usuários, incluindo operações como criação, atualização e exclusão de usuários.

**9\. Listagem de todos os projetos**  
    Implementar um endpoint para listar todos os projetos, independentemente do usuário associado

**10\. Alteração de prioridade de tarefa**  
    Permitir a alteração da prioridade de uma tarefa após sua criação nos dará maior flexibilidade na gestão das tarefas, permitindo que os usuários ajustem as prioridades conforme as mudanças de prioridade do negócio.

**11\. Reações às observações**  
    Adicionar a capacidade de os usuários reagirem a um comentário ou tarefa proporcionará uma forma mais rica de comunicação e colaboração entre os membros da equipe.

**12\. Anexos**  
    Implementar a possibilidade de anexar arquivos às tarefas ou projetos permitirá o compartilhamento de documentos e recursos relevantes diretamente no contexto das tarefas.

**13\. Seguir tarefas ou projetos**  
    Adicionar a funcionalidade de "seguir" uma tarefa ou projeto permitirá que os usuários recebam atualizações e notificações sobre atividades específicas que lhes interessem.

**14\. Seleção de período nos relatórios de desempenho**  
    Permitir que o usuário selecione o número de dias para o relatório de desempenho tornará essa funcionalidade mais flexível e adaptável às necessidades específicas de análise do usuário.

---

## Fase 3 - possíveis melhoraria no projeto

1\. **Utilizar Redis para consultas frequentes**  
    implementar o Redis para fazer cache de dados frequentemente acessados, o que poderia levar a uma significando diminuição da latençia para certos endpoints

2\. **Implementar um pipeline de CI/CD**  
    Estabelecer um pipeline de integração e entrega contínua (CI/CD) para garantir a qualidade das entregas em relação à execução da aplicação, realização de testes de integração e unitários, conformidade com os padrões de codificação estabelecidos e implantação automática.

3. **Utilizar Kubernetes para implantação**  
       Implementar o Kubernetes para automatizar a implantação em produção, gerenciar a orquestração da aplicação e garantir escalabilidade.

4\. **Adotar a arquitetura CQRS**  
    Considerar a adoção da arquitetura Command Query Responsibility Segregation (CQRS) para separar as preocupações de leitura e escrita, permitindo a escala independente desses dois aspectos.

5\. **Implementar monitoramento e logging**  
    Introduzir ferramentas de monitoramento e logging para acompanhar o desempenho da aplicação, identificar possíveis problemas e garantir uma operação contínua e eficiente.

6\. **Aplicar princípios de segurança**  
    Reforçar a segurança da aplicação implementando práticas recomendadas, como autenticação e autorização, criptografia e proteção contra ataques comuns, como injeção de SQL e cross-site scripting (XSS).

7\. **Realizar análise de desempenho**  
    Realizar análises periódicas de desempenho para identificar gargalos e áreas de melhoria, otimizando consultas de banco de dados, algoritmos e processos para garantir uma experiência do usuário mais rápida e eficiente.
