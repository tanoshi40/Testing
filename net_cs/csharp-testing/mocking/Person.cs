using Bogus;

namespace mocking;

/// <summary>
/// Represents a person with personal details and address information.
/// </summary>
public class Person : IEquatable<Person>
{
    /// <summary>
    /// Gets the unique identifier of the person.
    /// </summary>
    public Guid Id { get; }

    /// <summary>
    /// Gets or sets the first name of the person.
    /// </summary>
    public string FirstName { get; }

    /// <summary>
    /// Gets or sets the last name of the person.
    /// </summary>
    public string LastName { get; }

    /// <summary>
    /// Represents a person's street address.
    /// </summary>
    public string Street { get; }

    /// <summary>
    /// Represents a person with address information, including city.
    /// </summary>
    public string City { get; }

    /// <summary>
    /// Represents a person's state.
    /// </summary>
    /// <remarks>
    /// The state information of a person is typically used to specify the state or province
    /// in which the person resides.
    /// </remarks>
    public string State { get; }

    /// <summary>
    /// Gets the postal code of a person's address.
    /// </summary>
    public string PostalCode { get; }

    /// <summary>
    /// Represents a person.
    /// </summary>
    public Person(Guid id, string firstName, string lastName, string street, string city, string state,
        string postalCode)
    {
        Id = id;
        FirstName = firstName;
        LastName = lastName;
        Street = street;
        City = city;
        State = state;
        PostalCode = postalCode;
    }

    /// <summary>
    /// Represents a person with personal and address information.
    /// </summary>
    public Person(string firstName, string lastName, string street, string city, string state, string postalCode)
        : this(Guid.NewGuid(), firstName, lastName, street, city, state, postalCode)
    {
    }

    /// <summary>
    /// Represents a person with personal information and address details.
    /// </summary>
    public Person(Person other)
        : this(other.Id, other.FirstName, other.LastName, other.Street, other.City, other.State, other.PostalCode)
    {
    }

    /// <summary>
    /// Creates a new instance of the Person class with the same personal and address information as the current person.
    /// </summary>
    /// <returns>A new instance of the Person class with the same personal and address information.</returns>
    public Person Copy() => new(this);

    /// <summary>
    /// Returns a string that represents the current Person object.
    /// The string contains the ID, name, and address of the person.
    /// </summary>
    /// <returns>A string that represents the current Person object.</returns>
    public override string ToString()
    {
        return $"ID: {Id}\nName: {FirstName} {LastName}\nAddress: {Street}, {City}, {State}, {PostalCode}";
    }

    /// <summary>
    /// Generates a list of Person objects with random data.
    /// </summary>
    /// <param name="n">The number of Person objects to generate.</param>
    /// <returns>A list of Person objects.</returns>
    public static List<Person> GeneratePersons(int n)
    {
        var testUsers = new Faker<Person>()
            .CustomInstantiator(f => new Person(
                firstName: f.Name.FirstName(),
                lastName: f.Name.LastName(),
                street: f.Address.StreetAddress(),
                city: f.Address.City(),
                state: f.Address.State(),
                postalCode: f.Address.ZipCode()
            ))
            .Generate(n);

        return testUsers;
    }

    /// <summary>
    /// Determines whether the current instance of <see cref="Person"/> is equal to another instance.
    /// </summary>
    /// <param name="other">The <see cref="Person"/> to compare with the current instance.</param>
    /// <returns>
    /// <c>true</c> if the current instance is equal to the <paramref name="other"/> instance; otherwise, <c>false</c>.
    /// </returns>
    public bool Equals(Person? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return Id.Equals(other.Id)
               && FirstName == other.FirstName
               && LastName == other.LastName
               && Street == other.Street
               && City == other.City
               && State == other.State
               && PostalCode == other.PostalCode;
    }

    /// <summary>
    /// Determines whether the current Person object is equal to another Person object.
    /// </summary>
    /// <param name="other">The Person object to compare with the current object.</param>
    /// <returns>True if the current object is equal to the other object; otherwise, false.</returns>
    public override bool Equals(object? other)
    {
        return ReferenceEquals(this, other) || other is Person otherPerson && Equals(otherPerson);
    }

    /// <summary>
    /// Calculates and returns the hash code for the current Person instance.
    /// </summary>
    /// <remarks>
    /// The hash code is computed using the <see cref="Id"/>, <see cref="FirstName"/>, <see cref="LastName"/>,
    /// <see cref="Street"/>, <see cref="City"/>, <see cref="State"/>, and <see cref="PostalCode"/> properties of the Person object.
    /// </remarks>
    /// <returns>The hash code value for the current Person instance.</returns>
    public override int GetHashCode()
    {
        return HashCode.Combine(Id, FirstName, LastName, Street, City, State, PostalCode);
    }

    /// <summary>
    /// Represents a person with their personal details and address.
    /// </summary>
    public static bool operator ==(Person? left, Person? right)
    {
        return Equals(left, right);
    }

    /// <summary>
    /// Represents a person with various properties such as ID, first name, last name, address, etc.
    /// </summary>
    public static bool operator !=(Person? left, Person? right)
    {
        return !Equals(left, right);
    }
}