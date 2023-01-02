using PatternRecogniser.Models;

namespace PatternRecogniser.Messages.CutedModels
{
    public static class CutedModels
    {
        public static object CutedExtendModel(ExtendedModel ex)
        {
            return new
            {
                ex.extendedModelId,
                ex.name,
                ex.userLogin,
                ex.distribution,
                ex.num_classes
            };
        }

    }
}
