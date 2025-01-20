using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaSys.Metadata.Models.WorkflowModel.StepKinds
{
    public sealed class MessageStepKind : WorkflowStepKindBase
    {
        public override Guid Uid => Guid.Parse("{E192C288-154E-4778-B92C-64DA27279796}");
        public override string Name => "message";
    }
}
