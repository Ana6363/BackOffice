using BackOffice.Domain.Shared;

namespace BackOffice.Domain.OperationType;
public class OperationTypeId : EntityId 
{
    //public string Id {get;}
   /* private string operationTypeId { get;}

    public OperationTypeId(String value) : base(value)
    {
        this.operationTypeId = value;
    }
        
    override
    public String AsString(){
        return operationTypeId;     
    }
    override
    public int GetHashCode(){
        return operationTypeId.GetHashCode();
    }

    override
    public bool Equals(Object obj)
    {
        if (obj == null || GetType() != obj.GetType())
        {
            return false;
        }

        OperationTypeId operationType = (OperationTypeId)obj;
        return operationTypeId == operationType.operationTypeId;
    }

    
    protected override object CreateFromString(string text) 
    {
            
            if (string.IsNullOrWhiteSpace(text))
            {
                throw new ArgumentException("Value cannot be null or empty", nameof(text));
            }

            return new String(text);
    }
    */


    private readonly string idValue;

    public OperationTypeId(string value) : base(value)
    {
        if (string.IsNullOrEmpty(value))
        {
            throw new ArgumentException("ID cannot be null or empty.", nameof(value));
        }

        this.idValue = value;
    }

    public static OperationTypeId NewId()
    {
        return new OperationTypeId(Guid.NewGuid().ToString());
    }

    public override string AsString()
    {
        return this.idValue;
    }

    public override bool Equals(object obj)
    {
        if (obj is OperationTypeId other)
        {
            return this.idValue == other.idValue;
        }

        return false;
    }

    public override int GetHashCode()
    {
        return idValue.GetHashCode();
    }


    protected override object CreateFromString(string text) 
        {
            return new String(text);
        }
    
    public static OperationTypeId FromString(string id)
    {
        return new OperationTypeId(id);
    }

    

}