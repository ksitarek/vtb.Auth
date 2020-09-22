using NUnit.Framework;
using System;

namespace vtb.Auth.Tenant.Tests
{
    public class ValueTenantIdProviderTests
    {
        [Test]
        public void Will_Create_Instance_With_Preset_Value()
        {
            var tenantId = Guid.NewGuid();
            var vtp = new ValueTenantIdProvider(tenantId);
            Assert.AreEqual(tenantId, vtp.TenantId);
        }

        [Test]
        public void Will_Create_Instance_Without_Preset_Value()
        {
            var vtp = new ValueTenantIdProvider();
            Assert.AreEqual(Guid.Empty, vtp.TenantId);
        }

        [Test]
        public void Will_Allow_To_Set_TenantId_Manually_On_Instance_Without_Preset()
        {
            var tenantId = Guid.NewGuid();

            var vtp = new ValueTenantIdProvider();
            vtp.TenantId = tenantId;

            Assert.AreEqual(tenantId, vtp.TenantId);
        }

        [Test]
        public void Will_Not_Allow_To_Set_TenantId_Multiple_Times_On_Instance_Without_Preset()
        {
            var tenantId = Guid.NewGuid();

            var vtp = new ValueTenantIdProvider();
            vtp.TenantId = tenantId;
            Assert.Throws<InvalidOperationException>(() => vtp.TenantId = Guid.NewGuid());
        }

        [Test]
        public void Will_Not_Allow_To_Change_TenantId_On_Instance_With_Preset_Value()
        {
            var tenantId = Guid.NewGuid();

            var vtp = new ValueTenantIdProvider(tenantId);
            Assert.Throws<InvalidOperationException>(() => vtp.TenantId = Guid.NewGuid());
        }
    }
}