using Cassandra;
using System;

namespace Crimes.Cassandra
{
    public class SetUp
    {
        public ISession SetUpCassandra()
        {
            Cluster cluster = Cluster.Builder()
                .AddContactPoints("35.158.163.178", "35.158.217.185", "35.158.151.241")
                .WithAuthProvider(new PlainTextAuthProvider("iccassandra", "c24a0e043b2f439225898c5bdcb61a49"))
                .WithPort(9042)
                .WithLoadBalancingPolicy(new DCAwareRoundRobinPolicy("AWS_VPC_EU_CENTRAL_1"))
                .Build();

            ISession session = cluster.Connect();

            session.CreateKeyspaceIfNotExists("demo");
            session.ChangeKeyspace("demo");
            return session;
        }
    }
}
