using System.Collections.Generic;
using System.Collections.Concurrent;
using System;

public struct PseudoSequence : IEqualityComparer<PseudoSequence>
{
    // index of sequence 
    public int SequenceIndex;
    // start transaction
    public int TransactionIndex;
    // start item index
    public int ItemIndex;

    // valid sequence (not beyond bounds)
    public bool IsValid 
    {
        get { return SequenceIndex >= 0; }
    }
    
    public void Init(int SequenceIndex, int TransactionIndex, int ItemIndex)
    {
        this.SequenceIndex = SequenceIndex;
        this.TransactionIndex = TransactionIndex;
        this.ItemIndex = ItemIndex;
    }

    /* points to next item in the sequence, if currently
    pointing to the last item, IsValid set to false */
    public void PointToNextItem(Sequence seq)
    {
        if (ItemIndex + 1 >= seq[TransactionIndex].NumItems)
        {
            if (TransactionIndex + 1 >= seq.NumTransactions)
                SequenceIndex = -1; // invalid sequence

            TransactionIndex++;
            ItemIndex = 0;
        }
        else 
        {
            ItemIndex++;
        }
    }

    public bool Equals(PseudoSequence ps1, PseudoSequence ps2)
    {
        return ps1.SequenceIndex == ps2.SequenceIndex &&
            ps1.ItemIndex == ps2.TransactionIndex &&
            ps1.ItemIndex == ps2.TransactionIndex;
    }

    public int GetHashCode(PseudoSequence ps)
    {
        // multiplying by 89 supposedly provides good distribution
        return (ps.ItemIndex * 89) | (ps.TransactionIndex * 89) | (ps.SequenceIndex * 89);
    }
}
