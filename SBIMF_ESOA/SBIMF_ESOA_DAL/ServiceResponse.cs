using System;
using System.Reflection;
using System.ServiceModel;

namespace SBIMF_ESOA_DAL
{
    public class ServiceResponse
    {
        public object GetServiceResponse(string MethodName,params object[] arrayofParameters)
        {
            
                ChannelFactory<SBIMFSERVICE.ISBIMFSERVICE> myFactory;
                myFactory = new ChannelFactory<SBIMFSERVICE.ISBIMFSERVICE>("basicHttpBinding", new EndpointAddress("http://192.168.26.60/SBIMFSERVICE.svc/Wshttp"));
                var  oProxy = myFactory.CreateChannel();
                // commented out version that does same call without reflection
                // oProxy.GetData(3)
                Type oType = oProxy.GetType();
                MethodInfo oMeth = oType.GetMethod(MethodName);
                object[] @params = arrayofParameters;
                object sResults;
                sResults = oMeth.Invoke(oProxy, BindingFlags.Public | BindingFlags.InvokeMethod, null/* TODO Change to default(_) if this is not a reference type */, @params, System.Globalization.CultureInfo.CurrentCulture);

            return sResults;
        }
    }
}
