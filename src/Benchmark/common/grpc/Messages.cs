// Generated by the protocol buffer compiler.  DO NOT EDIT!
// source: messages.proto
#pragma warning disable 1591, 0612, 3021
#region Designer generated code

using pb = global::Google.Protobuf;
using pbc = global::Google.Protobuf.Collections;
using pbr = global::Google.Protobuf.Reflection;
using scg = global::System.Collections.Generic;
namespace Benchmark {

  /// <summary>Holder for reflection information generated from messages.proto</summary>
  public static partial class MessagesReflection {

    #region Descriptor
    /// <summary>File descriptor for messages.proto</summary>
    public static pbr::FileDescriptor Descriptor {
      get { return descriptor; }
    }
    private static pbr::FileDescriptor descriptor;

    static MessagesReflection() {
      byte[] descriptorData = global::System.Convert.FromBase64String(
          string.Concat(
            "Cg5tZXNzYWdlcy5wcm90bxIJYmVuY2htYXJrIg8KDU1lbW9yeVJlcXVlc3Qi",
            "1gEKEEJlbmNobWFya1JlcXVlc3QSCgoCaWQYASABKAkSJwoJQWxnb3JpdGht",
            "GAIgASgOMhQuYmVuY2htYXJrLkFsZ29yaXRobRIWCg5FcnJvclRvbGVyYW5j",
            "ZRgDIAEoARIMCgRGaWxlGAQgASgJEg8KB1N1cHBvcnQYBSABKAESEgoKU2Ft",
            "cGxlU2l6ZRgGIAEoBRIJCgFLGAcgASgFEg8KB1VzZVRvcEsYCCABKAgSDgoG",
            "REJTaXplGAkgASgFEhYKDlNodXRkb3duU2VydmVyGAogASgIIioKC01lbW9y",
            "eVJlcGx5EhsKE01lbW9yeVVzZ2FnZUluQnl0ZXMYASABKAMi3QEKDkJlbmNo",
            "bWFya1JlcGx5Eg8KB21lc3NhZ2UYASABKAkSFwoPU2VxdWVuY2VzSW5Kc29u",
            "GAMgASgJEhEKCUl0ZXJhdGlvbhgEIAEoBRIaChJOclByb2Nlc3NlZFJlY29y",
            "ZHMYBSABKAUSDQoFRXJyb3IYBiABKAESHAoUQmF0Y2hSdW50aW1lSW5NaWxs",
            "aXMYByABKAMSHAoUVG90YWxSdW50aW1lSW5NaWxsaXMYCCABKAMSJwoJUmVw",
            "bHlUeXBlGAkgASgOMhQuYmVuY2htYXJrLlJlcGx5VHlwZSo7CglBbGdvcml0",
            "aG0SDwoLSVByZWZpeFNwYW4QABIOCgpQcmVmaXhTcGFuEAESDQoJVW5kZWZp",
            "bmVkEAIqJAoJUmVwbHlUeXBlEgwKCENvbXBsZXRlEAASCQoFQmF0Y2gQATKg",
            "AQoLQmVuY2htYXJrZXISSgoMcnVuQmVuY2htYXJrEhsuYmVuY2htYXJrLkJl",
            "bmNobWFya1JlcXVlc3QaGS5iZW5jaG1hcmsuQmVuY2htYXJrUmVwbHkiADAB",
            "EkUKDW1vbml0b3JNZW1vcnkSGC5iZW5jaG1hcmsuTWVtb3J5UmVxdWVzdBoW",
            "LmJlbmNobWFyay5NZW1vcnlSZXBseSIAMAFiBnByb3RvMw=="));
      descriptor = pbr::FileDescriptor.FromGeneratedCode(descriptorData,
          new pbr::FileDescriptor[] { },
          new pbr::GeneratedClrTypeInfo(new[] {typeof(global::Benchmark.Algorithm), typeof(global::Benchmark.ReplyType), }, new pbr::GeneratedClrTypeInfo[] {
            new pbr::GeneratedClrTypeInfo(typeof(global::Benchmark.MemoryRequest), global::Benchmark.MemoryRequest.Parser, null, null, null, null),
            new pbr::GeneratedClrTypeInfo(typeof(global::Benchmark.BenchmarkRequest), global::Benchmark.BenchmarkRequest.Parser, new[]{ "Id", "Algorithm", "ErrorTolerance", "File", "Support", "SampleSize", "K", "UseTopK", "DBSize", "ShutdownServer" }, null, null, null),
            new pbr::GeneratedClrTypeInfo(typeof(global::Benchmark.MemoryReply), global::Benchmark.MemoryReply.Parser, new[]{ "MemoryUsgageInBytes" }, null, null, null),
            new pbr::GeneratedClrTypeInfo(typeof(global::Benchmark.BenchmarkReply), global::Benchmark.BenchmarkReply.Parser, new[]{ "Message", "SequencesInJson", "Iteration", "NrProcessedRecords", "Error", "BatchRuntimeInMillis", "TotalRuntimeInMillis", "ReplyType" }, null, null, null)
          }));
    }
    #endregion

  }
  #region Enums
  public enum Algorithm {
    [pbr::OriginalName("ProSecCo")] ProSecCo = 0,
    [pbr::OriginalName("PrefixSpan")] PrefixSpan = 1,
    [pbr::OriginalName("Undefined")] Undefined = 2,
  }

  public enum ReplyType {
    [pbr::OriginalName("Complete")] Complete = 0,
    [pbr::OriginalName("Batch")] Batch = 1,
  }

  #endregion

  #region Messages
  public sealed partial class MemoryRequest : pb::IMessage<MemoryRequest> {
    private static readonly pb::MessageParser<MemoryRequest> _parser = new pb::MessageParser<MemoryRequest>(() => new MemoryRequest());
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pb::MessageParser<MemoryRequest> Parser { get { return _parser; } }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pbr::MessageDescriptor Descriptor {
      get { return global::Benchmark.MessagesReflection.Descriptor.MessageTypes[0]; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    pbr::MessageDescriptor pb::IMessage.Descriptor {
      get { return Descriptor; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public MemoryRequest() {
      OnConstruction();
    }

    partial void OnConstruction();

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public MemoryRequest(MemoryRequest other) : this() {
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public MemoryRequest Clone() {
      return new MemoryRequest(this);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override bool Equals(object other) {
      return Equals(other as MemoryRequest);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public bool Equals(MemoryRequest other) {
      if (ReferenceEquals(other, null)) {
        return false;
      }
      if (ReferenceEquals(other, this)) {
        return true;
      }
      return true;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override int GetHashCode() {
      int hash = 1;
      return hash;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override string ToString() {
      return pb::JsonFormatter.ToDiagnosticString(this);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void WriteTo(pb::CodedOutputStream output) {
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int CalculateSize() {
      int size = 0;
      return size;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(MemoryRequest other) {
      if (other == null) {
        return;
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(pb::CodedInputStream input) {
      uint tag;
      while ((tag = input.ReadTag()) != 0) {
        switch(tag) {
          default:
            input.SkipLastField();
            break;
        }
      }
    }

  }

  public sealed partial class BenchmarkRequest : pb::IMessage<BenchmarkRequest> {
    private static readonly pb::MessageParser<BenchmarkRequest> _parser = new pb::MessageParser<BenchmarkRequest>(() => new BenchmarkRequest());
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pb::MessageParser<BenchmarkRequest> Parser { get { return _parser; } }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pbr::MessageDescriptor Descriptor {
      get { return global::Benchmark.MessagesReflection.Descriptor.MessageTypes[1]; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    pbr::MessageDescriptor pb::IMessage.Descriptor {
      get { return Descriptor; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public BenchmarkRequest() {
      OnConstruction();
    }

    partial void OnConstruction();

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public BenchmarkRequest(BenchmarkRequest other) : this() {
      id_ = other.id_;
      algorithm_ = other.algorithm_;
      errorTolerance_ = other.errorTolerance_;
      file_ = other.file_;
      support_ = other.support_;
      sampleSize_ = other.sampleSize_;
      k_ = other.k_;
      useTopK_ = other.useTopK_;
      dBSize_ = other.dBSize_;
      shutdownServer_ = other.shutdownServer_;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public BenchmarkRequest Clone() {
      return new BenchmarkRequest(this);
    }

    /// <summary>Field number for the "id" field.</summary>
    public const int IdFieldNumber = 1;
    private string id_ = "";
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public string Id {
      get { return id_; }
      set {
        id_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
      }
    }

    /// <summary>Field number for the "Algorithm" field.</summary>
    public const int AlgorithmFieldNumber = 2;
    private global::Benchmark.Algorithm algorithm_ = 0;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public global::Benchmark.Algorithm Algorithm {
      get { return algorithm_; }
      set {
        algorithm_ = value;
      }
    }

    /// <summary>Field number for the "ErrorTolerance" field.</summary>
    public const int ErrorToleranceFieldNumber = 3;
    private double errorTolerance_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public double ErrorTolerance {
      get { return errorTolerance_; }
      set {
        errorTolerance_ = value;
      }
    }

    /// <summary>Field number for the "File" field.</summary>
    public const int FileFieldNumber = 4;
    private string file_ = "";
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public string File {
      get { return file_; }
      set {
        file_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
      }
    }

    /// <summary>Field number for the "Support" field.</summary>
    public const int SupportFieldNumber = 5;
    private double support_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public double Support {
      get { return support_; }
      set {
        support_ = value;
      }
    }

    /// <summary>Field number for the "SampleSize" field.</summary>
    public const int SampleSizeFieldNumber = 6;
    private int sampleSize_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int SampleSize {
      get { return sampleSize_; }
      set {
        sampleSize_ = value;
      }
    }

    /// <summary>Field number for the "K" field.</summary>
    public const int KFieldNumber = 7;
    private int k_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int K {
      get { return k_; }
      set {
        k_ = value;
      }
    }

    /// <summary>Field number for the "UseTopK" field.</summary>
    public const int UseTopKFieldNumber = 8;
    private bool useTopK_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public bool UseTopK {
      get { return useTopK_; }
      set {
        useTopK_ = value;
      }
    }

    /// <summary>Field number for the "DBSize" field.</summary>
    public const int DBSizeFieldNumber = 9;
    private int dBSize_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int DBSize {
      get { return dBSize_; }
      set {
        dBSize_ = value;
      }
    }

    /// <summary>Field number for the "ShutdownServer" field.</summary>
    public const int ShutdownServerFieldNumber = 10;
    private bool shutdownServer_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public bool ShutdownServer {
      get { return shutdownServer_; }
      set {
        shutdownServer_ = value;
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override bool Equals(object other) {
      return Equals(other as BenchmarkRequest);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public bool Equals(BenchmarkRequest other) {
      if (ReferenceEquals(other, null)) {
        return false;
      }
      if (ReferenceEquals(other, this)) {
        return true;
      }
      if (Id != other.Id) return false;
      if (Algorithm != other.Algorithm) return false;
      if (ErrorTolerance != other.ErrorTolerance) return false;
      if (File != other.File) return false;
      if (Support != other.Support) return false;
      if (SampleSize != other.SampleSize) return false;
      if (K != other.K) return false;
      if (UseTopK != other.UseTopK) return false;
      if (DBSize != other.DBSize) return false;
      if (ShutdownServer != other.ShutdownServer) return false;
      return true;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override int GetHashCode() {
      int hash = 1;
      if (Id.Length != 0) hash ^= Id.GetHashCode();
      if (Algorithm != 0) hash ^= Algorithm.GetHashCode();
      if (ErrorTolerance != 0D) hash ^= ErrorTolerance.GetHashCode();
      if (File.Length != 0) hash ^= File.GetHashCode();
      if (Support != 0D) hash ^= Support.GetHashCode();
      if (SampleSize != 0) hash ^= SampleSize.GetHashCode();
      if (K != 0) hash ^= K.GetHashCode();
      if (UseTopK != false) hash ^= UseTopK.GetHashCode();
      if (DBSize != 0) hash ^= DBSize.GetHashCode();
      if (ShutdownServer != false) hash ^= ShutdownServer.GetHashCode();
      return hash;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override string ToString() {
      return pb::JsonFormatter.ToDiagnosticString(this);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void WriteTo(pb::CodedOutputStream output) {
      if (Id.Length != 0) {
        output.WriteRawTag(10);
        output.WriteString(Id);
      }
      if (Algorithm != 0) {
        output.WriteRawTag(16);
        output.WriteEnum((int) Algorithm);
      }
      if (ErrorTolerance != 0D) {
        output.WriteRawTag(25);
        output.WriteDouble(ErrorTolerance);
      }
      if (File.Length != 0) {
        output.WriteRawTag(34);
        output.WriteString(File);
      }
      if (Support != 0D) {
        output.WriteRawTag(41);
        output.WriteDouble(Support);
      }
      if (SampleSize != 0) {
        output.WriteRawTag(48);
        output.WriteInt32(SampleSize);
      }
      if (K != 0) {
        output.WriteRawTag(56);
        output.WriteInt32(K);
      }
      if (UseTopK != false) {
        output.WriteRawTag(64);
        output.WriteBool(UseTopK);
      }
      if (DBSize != 0) {
        output.WriteRawTag(72);
        output.WriteInt32(DBSize);
      }
      if (ShutdownServer != false) {
        output.WriteRawTag(80);
        output.WriteBool(ShutdownServer);
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int CalculateSize() {
      int size = 0;
      if (Id.Length != 0) {
        size += 1 + pb::CodedOutputStream.ComputeStringSize(Id);
      }
      if (Algorithm != 0) {
        size += 1 + pb::CodedOutputStream.ComputeEnumSize((int) Algorithm);
      }
      if (ErrorTolerance != 0D) {
        size += 1 + 8;
      }
      if (File.Length != 0) {
        size += 1 + pb::CodedOutputStream.ComputeStringSize(File);
      }
      if (Support != 0D) {
        size += 1 + 8;
      }
      if (SampleSize != 0) {
        size += 1 + pb::CodedOutputStream.ComputeInt32Size(SampleSize);
      }
      if (K != 0) {
        size += 1 + pb::CodedOutputStream.ComputeInt32Size(K);
      }
      if (UseTopK != false) {
        size += 1 + 1;
      }
      if (DBSize != 0) {
        size += 1 + pb::CodedOutputStream.ComputeInt32Size(DBSize);
      }
      if (ShutdownServer != false) {
        size += 1 + 1;
      }
      return size;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(BenchmarkRequest other) {
      if (other == null) {
        return;
      }
      if (other.Id.Length != 0) {
        Id = other.Id;
      }
      if (other.Algorithm != 0) {
        Algorithm = other.Algorithm;
      }
      if (other.ErrorTolerance != 0D) {
        ErrorTolerance = other.ErrorTolerance;
      }
      if (other.File.Length != 0) {
        File = other.File;
      }
      if (other.Support != 0D) {
        Support = other.Support;
      }
      if (other.SampleSize != 0) {
        SampleSize = other.SampleSize;
      }
      if (other.K != 0) {
        K = other.K;
      }
      if (other.UseTopK != false) {
        UseTopK = other.UseTopK;
      }
      if (other.DBSize != 0) {
        DBSize = other.DBSize;
      }
      if (other.ShutdownServer != false) {
        ShutdownServer = other.ShutdownServer;
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(pb::CodedInputStream input) {
      uint tag;
      while ((tag = input.ReadTag()) != 0) {
        switch(tag) {
          default:
            input.SkipLastField();
            break;
          case 10: {
            Id = input.ReadString();
            break;
          }
          case 16: {
            algorithm_ = (global::Benchmark.Algorithm) input.ReadEnum();
            break;
          }
          case 25: {
            ErrorTolerance = input.ReadDouble();
            break;
          }
          case 34: {
            File = input.ReadString();
            break;
          }
          case 41: {
            Support = input.ReadDouble();
            break;
          }
          case 48: {
            SampleSize = input.ReadInt32();
            break;
          }
          case 56: {
            K = input.ReadInt32();
            break;
          }
          case 64: {
            UseTopK = input.ReadBool();
            break;
          }
          case 72: {
            DBSize = input.ReadInt32();
            break;
          }
          case 80: {
            ShutdownServer = input.ReadBool();
            break;
          }
        }
      }
    }

  }

  public sealed partial class MemoryReply : pb::IMessage<MemoryReply> {
    private static readonly pb::MessageParser<MemoryReply> _parser = new pb::MessageParser<MemoryReply>(() => new MemoryReply());
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pb::MessageParser<MemoryReply> Parser { get { return _parser; } }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pbr::MessageDescriptor Descriptor {
      get { return global::Benchmark.MessagesReflection.Descriptor.MessageTypes[2]; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    pbr::MessageDescriptor pb::IMessage.Descriptor {
      get { return Descriptor; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public MemoryReply() {
      OnConstruction();
    }

    partial void OnConstruction();

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public MemoryReply(MemoryReply other) : this() {
      memoryUsgageInBytes_ = other.memoryUsgageInBytes_;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public MemoryReply Clone() {
      return new MemoryReply(this);
    }

    /// <summary>Field number for the "MemoryUsgageInBytes" field.</summary>
    public const int MemoryUsgageInBytesFieldNumber = 1;
    private long memoryUsgageInBytes_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public long MemoryUsgageInBytes {
      get { return memoryUsgageInBytes_; }
      set {
        memoryUsgageInBytes_ = value;
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override bool Equals(object other) {
      return Equals(other as MemoryReply);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public bool Equals(MemoryReply other) {
      if (ReferenceEquals(other, null)) {
        return false;
      }
      if (ReferenceEquals(other, this)) {
        return true;
      }
      if (MemoryUsgageInBytes != other.MemoryUsgageInBytes) return false;
      return true;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override int GetHashCode() {
      int hash = 1;
      if (MemoryUsgageInBytes != 0L) hash ^= MemoryUsgageInBytes.GetHashCode();
      return hash;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override string ToString() {
      return pb::JsonFormatter.ToDiagnosticString(this);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void WriteTo(pb::CodedOutputStream output) {
      if (MemoryUsgageInBytes != 0L) {
        output.WriteRawTag(8);
        output.WriteInt64(MemoryUsgageInBytes);
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int CalculateSize() {
      int size = 0;
      if (MemoryUsgageInBytes != 0L) {
        size += 1 + pb::CodedOutputStream.ComputeInt64Size(MemoryUsgageInBytes);
      }
      return size;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(MemoryReply other) {
      if (other == null) {
        return;
      }
      if (other.MemoryUsgageInBytes != 0L) {
        MemoryUsgageInBytes = other.MemoryUsgageInBytes;
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(pb::CodedInputStream input) {
      uint tag;
      while ((tag = input.ReadTag()) != 0) {
        switch(tag) {
          default:
            input.SkipLastField();
            break;
          case 8: {
            MemoryUsgageInBytes = input.ReadInt64();
            break;
          }
        }
      }
    }

  }

  public sealed partial class BenchmarkReply : pb::IMessage<BenchmarkReply> {
    private static readonly pb::MessageParser<BenchmarkReply> _parser = new pb::MessageParser<BenchmarkReply>(() => new BenchmarkReply());
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pb::MessageParser<BenchmarkReply> Parser { get { return _parser; } }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pbr::MessageDescriptor Descriptor {
      get { return global::Benchmark.MessagesReflection.Descriptor.MessageTypes[3]; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    pbr::MessageDescriptor pb::IMessage.Descriptor {
      get { return Descriptor; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public BenchmarkReply() {
      OnConstruction();
    }

    partial void OnConstruction();

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public BenchmarkReply(BenchmarkReply other) : this() {
      message_ = other.message_;
      sequencesInJson_ = other.sequencesInJson_;
      iteration_ = other.iteration_;
      nrProcessedRecords_ = other.nrProcessedRecords_;
      error_ = other.error_;
      batchRuntimeInMillis_ = other.batchRuntimeInMillis_;
      totalRuntimeInMillis_ = other.totalRuntimeInMillis_;
      replyType_ = other.replyType_;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public BenchmarkReply Clone() {
      return new BenchmarkReply(this);
    }

    /// <summary>Field number for the "message" field.</summary>
    public const int MessageFieldNumber = 1;
    private string message_ = "";
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public string Message {
      get { return message_; }
      set {
        message_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
      }
    }

    /// <summary>Field number for the "SequencesInJson" field.</summary>
    public const int SequencesInJsonFieldNumber = 3;
    private string sequencesInJson_ = "";
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public string SequencesInJson {
      get { return sequencesInJson_; }
      set {
        sequencesInJson_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
      }
    }

    /// <summary>Field number for the "Iteration" field.</summary>
    public const int IterationFieldNumber = 4;
    private int iteration_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int Iteration {
      get { return iteration_; }
      set {
        iteration_ = value;
      }
    }

    /// <summary>Field number for the "NrProcessedRecords" field.</summary>
    public const int NrProcessedRecordsFieldNumber = 5;
    private int nrProcessedRecords_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int NrProcessedRecords {
      get { return nrProcessedRecords_; }
      set {
        nrProcessedRecords_ = value;
      }
    }

    /// <summary>Field number for the "Error" field.</summary>
    public const int ErrorFieldNumber = 6;
    private double error_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public double Error {
      get { return error_; }
      set {
        error_ = value;
      }
    }

    /// <summary>Field number for the "BatchRuntimeInMillis" field.</summary>
    public const int BatchRuntimeInMillisFieldNumber = 7;
    private long batchRuntimeInMillis_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public long BatchRuntimeInMillis {
      get { return batchRuntimeInMillis_; }
      set {
        batchRuntimeInMillis_ = value;
      }
    }

    /// <summary>Field number for the "TotalRuntimeInMillis" field.</summary>
    public const int TotalRuntimeInMillisFieldNumber = 8;
    private long totalRuntimeInMillis_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public long TotalRuntimeInMillis {
      get { return totalRuntimeInMillis_; }
      set {
        totalRuntimeInMillis_ = value;
      }
    }

    /// <summary>Field number for the "ReplyType" field.</summary>
    public const int ReplyTypeFieldNumber = 9;
    private global::Benchmark.ReplyType replyType_ = 0;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public global::Benchmark.ReplyType ReplyType {
      get { return replyType_; }
      set {
        replyType_ = value;
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override bool Equals(object other) {
      return Equals(other as BenchmarkReply);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public bool Equals(BenchmarkReply other) {
      if (ReferenceEquals(other, null)) {
        return false;
      }
      if (ReferenceEquals(other, this)) {
        return true;
      }
      if (Message != other.Message) return false;
      if (SequencesInJson != other.SequencesInJson) return false;
      if (Iteration != other.Iteration) return false;
      if (NrProcessedRecords != other.NrProcessedRecords) return false;
      if (Error != other.Error) return false;
      if (BatchRuntimeInMillis != other.BatchRuntimeInMillis) return false;
      if (TotalRuntimeInMillis != other.TotalRuntimeInMillis) return false;
      if (ReplyType != other.ReplyType) return false;
      return true;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override int GetHashCode() {
      int hash = 1;
      if (Message.Length != 0) hash ^= Message.GetHashCode();
      if (SequencesInJson.Length != 0) hash ^= SequencesInJson.GetHashCode();
      if (Iteration != 0) hash ^= Iteration.GetHashCode();
      if (NrProcessedRecords != 0) hash ^= NrProcessedRecords.GetHashCode();
      if (Error != 0D) hash ^= Error.GetHashCode();
      if (BatchRuntimeInMillis != 0L) hash ^= BatchRuntimeInMillis.GetHashCode();
      if (TotalRuntimeInMillis != 0L) hash ^= TotalRuntimeInMillis.GetHashCode();
      if (ReplyType != 0) hash ^= ReplyType.GetHashCode();
      return hash;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override string ToString() {
      return pb::JsonFormatter.ToDiagnosticString(this);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void WriteTo(pb::CodedOutputStream output) {
      if (Message.Length != 0) {
        output.WriteRawTag(10);
        output.WriteString(Message);
      }
      if (SequencesInJson.Length != 0) {
        output.WriteRawTag(26);
        output.WriteString(SequencesInJson);
      }
      if (Iteration != 0) {
        output.WriteRawTag(32);
        output.WriteInt32(Iteration);
      }
      if (NrProcessedRecords != 0) {
        output.WriteRawTag(40);
        output.WriteInt32(NrProcessedRecords);
      }
      if (Error != 0D) {
        output.WriteRawTag(49);
        output.WriteDouble(Error);
      }
      if (BatchRuntimeInMillis != 0L) {
        output.WriteRawTag(56);
        output.WriteInt64(BatchRuntimeInMillis);
      }
      if (TotalRuntimeInMillis != 0L) {
        output.WriteRawTag(64);
        output.WriteInt64(TotalRuntimeInMillis);
      }
      if (ReplyType != 0) {
        output.WriteRawTag(72);
        output.WriteEnum((int) ReplyType);
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int CalculateSize() {
      int size = 0;
      if (Message.Length != 0) {
        size += 1 + pb::CodedOutputStream.ComputeStringSize(Message);
      }
      if (SequencesInJson.Length != 0) {
        size += 1 + pb::CodedOutputStream.ComputeStringSize(SequencesInJson);
      }
      if (Iteration != 0) {
        size += 1 + pb::CodedOutputStream.ComputeInt32Size(Iteration);
      }
      if (NrProcessedRecords != 0) {
        size += 1 + pb::CodedOutputStream.ComputeInt32Size(NrProcessedRecords);
      }
      if (Error != 0D) {
        size += 1 + 8;
      }
      if (BatchRuntimeInMillis != 0L) {
        size += 1 + pb::CodedOutputStream.ComputeInt64Size(BatchRuntimeInMillis);
      }
      if (TotalRuntimeInMillis != 0L) {
        size += 1 + pb::CodedOutputStream.ComputeInt64Size(TotalRuntimeInMillis);
      }
      if (ReplyType != 0) {
        size += 1 + pb::CodedOutputStream.ComputeEnumSize((int) ReplyType);
      }
      return size;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(BenchmarkReply other) {
      if (other == null) {
        return;
      }
      if (other.Message.Length != 0) {
        Message = other.Message;
      }
      if (other.SequencesInJson.Length != 0) {
        SequencesInJson = other.SequencesInJson;
      }
      if (other.Iteration != 0) {
        Iteration = other.Iteration;
      }
      if (other.NrProcessedRecords != 0) {
        NrProcessedRecords = other.NrProcessedRecords;
      }
      if (other.Error != 0D) {
        Error = other.Error;
      }
      if (other.BatchRuntimeInMillis != 0L) {
        BatchRuntimeInMillis = other.BatchRuntimeInMillis;
      }
      if (other.TotalRuntimeInMillis != 0L) {
        TotalRuntimeInMillis = other.TotalRuntimeInMillis;
      }
      if (other.ReplyType != 0) {
        ReplyType = other.ReplyType;
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(pb::CodedInputStream input) {
      uint tag;
      while ((tag = input.ReadTag()) != 0) {
        switch(tag) {
          default:
            input.SkipLastField();
            break;
          case 10: {
            Message = input.ReadString();
            break;
          }
          case 26: {
            SequencesInJson = input.ReadString();
            break;
          }
          case 32: {
            Iteration = input.ReadInt32();
            break;
          }
          case 40: {
            NrProcessedRecords = input.ReadInt32();
            break;
          }
          case 49: {
            Error = input.ReadDouble();
            break;
          }
          case 56: {
            BatchRuntimeInMillis = input.ReadInt64();
            break;
          }
          case 64: {
            TotalRuntimeInMillis = input.ReadInt64();
            break;
          }
          case 72: {
            replyType_ = (global::Benchmark.ReplyType) input.ReadEnum();
            break;
          }
        }
      }
    }

  }

  #endregion

}

#endregion Designer generated code
