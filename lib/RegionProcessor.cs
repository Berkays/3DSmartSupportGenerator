using System.Collections.Concurrent;
using System.Diagnostics;

public class RegionProcessor
{
    public MetricValue Step { get; init; }
    // Square blocks with MinRegionSize ^ 2 Area.
    private int MinRegionSize { get; init; }
    private static readonly MetricValue SupportBaseDimension = new(1, MetricUnit.Milimeter);

    public RegionProcessor(MetricValue step)
    {
        this.Step = step;

        this.MinRegionSize = (int)(SupportBaseDimension.value / (step.ConvertToUnit(SupportBaseDimension.unit)).value);
    }
    public RegionContainer ReduceTopology(OverhangTopology topology)
    {
        // ConcurrentBag<ValueTuple<int, int, int>> bag = new ConcurrentBag<(int, int, int)>();
        // FindRowSequences(intersections, modelXRank, bag);
        // MatchColumnSequences(intersections, modelYRank, bag);

        var kernel = new RegionKernel(this.MinRegionSize);

        return kernel.Compute(topology);
    }
    private void FindRowSequences(byte[,] intersections, int modelXRank, ConcurrentBag<(int, int, int)> bag)
    {
        int bound = intersections.GetUpperBound(1) + 1;
        Action<int> fn = new Action<int>((int i) =>
        {
            Span<byte> row = stackalloc byte[bound];

            // var r = intersections.GetRow(i);
            intersections.GetRowSpan(i, bound, row);
            // var rowSum = rowBuffer.ToArray().Select(n => (int)n).Sum();

            // if (rowSum < MinRegionSize)
            //     return;

            // Find first 1
            int start_index = row.IndexOf((byte)1);
            int end_index = row.LastIndexOf((byte)1);

            int sliceLength = end_index - start_index + 1;

            // Check sequence length
            if (sliceLength < MinRegionSize)
                return;

            int sequenceMaxPossibleRegionCount = sliceLength / MinRegionSize;
            Debug.Assert(sequenceMaxPossibleRegionCount > 0, "Max possible region count must be greater than 0 at this point");

            int sequenceRegionCount = 0;

            for (int ci = start_index; ci <= end_index - MinRegionSize && sequenceRegionCount < sequenceMaxPossibleRegionCount; ci++)
            {
                int ck = ci + 1;
                int sequenceLength = 0;
                for (; ck <= end_index; ck++)
                {
                    if (row[ck] == 0)
                    {
                        if (sequenceLength >= MinRegionSize)
                        {
                            bag.Add((i, ci, ck)); // Commit the row sequence(ci,ck)
                            sequenceRegionCount++;
                        }

                        sequenceLength = 0;

                        // Find next 1 in row
                        ci = ck + row.Slice(ck + 1, end_index - ck + 2).IndexOf((byte)1) + 1;
                        break;
                    }

                    sequenceLength++;
                }
            }
        });
        Parallel.For(0, modelXRank, fn);
    }

    private void MatchColumnSequences(byte[,] intersections, int modelYRank, ConcurrentBag<ValueTuple<int, int, int>> bag)
    {
        // int minIndex = bag.MinBy(n => n.Key).Key;
        // int maxIndex = bag.MaxBy(n => n.Key).Key;

        // if (maxIndex - minIndex < this.MinRegionSize)
        // return;

        // Check columns for all sequences
        ConcurrentBag<ValueTuple<int, int, int, bool>> b = new ConcurrentBag<(int, int, int, bool)>();

        foreach (var sequence in bag)
        {
            var (rowIndex, sequenceStart, sequenceEnd) = sequence;
            int sequenceLength = sequenceEnd - sequenceStart + 1;


            // Check column for single sequence byte



        }
        // for (int sequencePosition = sequenceStart; sequencePosition < sequenceEnd; sequencePosition++)
        // {
        //     int columnSequenceLength = 0;
        //     for (int i = rowIndex + 1; i < modelYRank; i++)
        //     {
        //         if (intersections[i, sequenceStart] == 0)
        //         {
        //             if (columnSequenceLength > MinRegionSize)
        //             {

        //             }

        //             break;
        //         }

        //         columnSequenceLength++;
        //     }
        // }

        //     for (int rowIndex = minIndex; rowIndex < maxIndex; rowIndex++)
        //     {
        //         // Sequences in the same row
        //         var sequencesInRow = bag[rowIndex];

        //         foreach (var sequence in sequencesInRow)
        //         {
        //             // rowSequence: (rowIndex, sequenceStart, sequenceEnd)
        //             var (_, sequenceStart, sequenceEnd) = sequence;
        //             var sequenceLength = sequenceEnd - sequenceStart + 1;

        //             for (int sQs = sequenceStart; sQs < sequenceEnd; sQs++)
        //             {

        //                 // Traverse columns
        //                 for (int i = rowIndex + 1; i < modelYRank; i++)
        //                 {
        //                     if (bag[i]
        //                 }
        //             }
        //         }
        //     }
    }
}