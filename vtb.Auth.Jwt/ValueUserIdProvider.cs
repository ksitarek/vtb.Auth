using System;

namespace vtb.Auth.Jwt
{
    public class ValueUserIdProvider : IUserIdProvider
    {
        private Guid? _userId = null;
        public Guid UserId
        {
            get
            {
                return _userId != null ? _userId.Value : Guid.Empty;
            }

            set
            {
                if (_userId != null)
                    throw new InvalidOperationException("UserId can be set only once.");

                _userId = value;
            }
        }

        public ValueUserIdProvider()
        {
            _userId = null;
        }

        public ValueUserIdProvider(Guid userId)
        {
            _userId = userId;
        }
    }
}
