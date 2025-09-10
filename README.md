Gerenciamento de Sessões com Redis e NoSQL (C# / .NET 8)
Este projeto implementa um sistema simples de gerenciamento de sessões de usuário para uma aplicação de e-commerce.
A ideia é combinar Redis (para cache de sessões) com um banco NoSQL (Cassandra), garantindo rapidez no login e menos carga no banco.
Como funciona
O usuário faz login informando seu ID.
O sistema verifica primeiro no Redis (cache).
Se não encontrar, busca no Cassandra, grava no cache com um prazo de 15 minutos e devolve a resposta.
Sempre que o login acontece, a data de último acesso do usuário é atualizada.
Classe usada para armazenar os dados:
public sealed record UserProfile
{
    public string Id { get; init; }
    public string Nome { get; init; }
    public string Email { get; init; }
    public DateTimeOffset UltimoAcesso { get; init; }
}
Como rodar (jeito fácil)
Requisitos: Docker + Docker Compose
docker compose up --build
Isso sobe três serviços:
Redis em localhost:6379
Cassandra em localhost:9042
API em http://localhost:8080 (com Swagger em /swagger)
Testando na prática
Inserir um usuário no Cassandra:
docker exec -it $(docker ps -qf "ancestor=cassandra:4.1") cqlsh \
  -e "INSERT INTO ecommerce.user_profile (id, nome, email, ultimo_acesso) VALUES ('u1','Ana','ana@example.com', toTimestamp(now()));"
Fazer login pela API:
curl -X POST http://localhost:8080/api/auth/login/u1
→ Primeira vez: dados vêm do Cassandra.
→ Próximas vezes: resposta rápida do Redis.
Testar pelo navegador:
Acesse http://localhost:8080/swagger e use o endpoint POST /api/auth/login/{userId}.
Estrutura do projeto
src/SessionApp/
  Controllers/AuthController.cs
  Models/UserProfile.cs
  Services/SessionService.cs
  Infra/RedisSessionCache.cs
  Infra/CassandraUserStore.cs
  Program.cs
docker-compose.yml
Dockerfile
Boas práticas aplicadas
Operações assíncronas (async/await)
Cache com TTL de 15 minutos
Atualização de último acesso sempre que o usuário loga
Conexões com Redis e Cassandra configuradas como singleton
Documentação via Swagger para facilitar os testes